using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif // #if DEBUG
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// I picked this class from somewhere named BroccoliProducts. It's a good class indeed, 
/// but I found this class did not support my requirements to use TripleDES with ECB mode, 
/// no padding (or padding with null). So I make some changes.
/// </summary>
namespace DurianBerryProducts
{

    /// <summary>
    /// Declaration of DESCrytography class
    /// </summary>
    public static class DESCrytography
    {

        /////////////////////////////////////////////////////////////
        // Nested classes

        /// <summary>
        /// Declaration of BLOCK8BYTE class
        /// </summary>
        internal class BLOCK8BYTE
        {

            /////////////////////////////////////////////////////////
            // Constants

            public const int BYTE_LENGTH = 8;

            /////////////////////////////////////////////////////////
            // Attributes

            internal byte[] m_data = new byte[BYTE_LENGTH];

            /////////////////////////////////////////////////////////
            // Operations

            public void Reset()
            {

                // Reset bytes
                Array.Clear(m_data, 0, BYTE_LENGTH);

            }

            public void Set(BLOCK8BYTE Source)
            {

                // Copy source data to this
                this.Set(Source.m_data, 0);

            }

            public void Set(byte[] buffer, int iOffset)
            {

                // Set contents by copying array
                Array.Copy(buffer, iOffset, m_data, 0, BYTE_LENGTH);

            }

            public void Xor(BLOCK8BYTE A, BLOCK8BYTE B)
            {

                // Set byte to A ^ B
                for (int iOffset = 0; iOffset < BYTE_LENGTH; iOffset++)
                    m_data[iOffset] = Convert.ToByte(A.m_data[iOffset] ^ B.m_data[iOffset]);

            }

            public void SetBit(int iByteOffset, int iBitOffset, bool bFlag)
            {

                // Compose mask
                byte mask = Convert.ToByte(1 << iBitOffset);
                if (((m_data[iByteOffset] & mask) == mask) != bFlag)
                    m_data[iByteOffset] ^= mask;

            }

            public bool GetBit(int iByteOffset, int iBitOffset)
            {

                // call sibling function
                return ((this.m_data[iByteOffset] >> iBitOffset) & 0x01) == 0x01;

            }

            public void ShiftLeftWrapped(BLOCK8BYTE S, int iBitShift)
            {

                // this shift is only applied to the first 32 bits, and parity bit is ignored

                // Declaration of local variables
                int iByteOffset = 0;
                bool bBit = false;

                // Copy byte and shift regardless
                for (iByteOffset = 0; iByteOffset < 4; iByteOffset++)
                    m_data[iByteOffset] = Convert.ToByte((S.m_data[iByteOffset] << iBitShift) & 0xFF);

                // if shifting by 1...
                if (iBitShift == 1)
                {

                    // repair bits on right of BYTE
                    for (iByteOffset = 0; iByteOffset < 3; iByteOffset++)
                    {

                        // get repairing bit offsets
                        bBit = S.GetBit(iByteOffset + 1, 7);
                        this.SetBit(iByteOffset, 1, bBit);

                    }

                    // wrap around the final bit
                    this.SetBit(3, 1, S.GetBit(0, 7));

                }
                else if (iBitShift == 2)
                {

                    // repair bits on right of BYTE
                    for (iByteOffset = 0; iByteOffset < 3; iByteOffset++)
                    {

                        // get repairing bit offsets
                        bBit = S.GetBit(iByteOffset + 1, 7);
                        this.SetBit(iByteOffset, 2, bBit);
                        bBit = S.GetBit(iByteOffset + 1, 6);
                        this.SetBit(iByteOffset, 1, bBit);

                    }

                    // wrap around the final bit
                    this.SetBit(3, 2, S.GetBit(0, 7));
                    this.SetBit(3, 1, S.GetBit(0, 6));

                }
#if DEBUG
                else
                    Debug.Assert(false);
#endif // #if DEBUG

            }

        }

        /// <summary>
        /// Declaration of KEY_SET class
        /// </summary>
        internal class KEY_SET
        {

            /////////////////////////////////////////////////////////
            // Constants

            public const int KEY_COUNT = 17;

            /////////////////////////////////////////////////////////
            // Attributes

            internal BLOCK8BYTE[] m_array;

            /////////////////////////////////////////////////////////
            // Construction

            internal KEY_SET()
            {

                // Create array
                m_array = new BLOCK8BYTE[KEY_COUNT];
                for (int i1 = 0; i1 < KEY_COUNT; i1++)
                    m_array[i1] = new BLOCK8BYTE();

            }

            /////////////////////////////////////////////////////////
            // Operations

            public BLOCK8BYTE GetAt(int iArrayOffset)
            {
                return m_array[iArrayOffset];
            }

        }

        /// <summary>
        /// Declaration of WORKING_SET class
        /// </summary>
        internal class WORKING_SET
        {

            /////////////////////////////////////////////////////////
            // Attributes

            internal BLOCK8BYTE IP = new BLOCK8BYTE();
            internal BLOCK8BYTE[] Ln = new BLOCK8BYTE[17];
            internal BLOCK8BYTE[] Rn = new BLOCK8BYTE[17];
            internal BLOCK8BYTE RnExpand = new BLOCK8BYTE();
            internal BLOCK8BYTE XorBlock = new BLOCK8BYTE();
            internal BLOCK8BYTE SBoxValues = new BLOCK8BYTE();
            internal BLOCK8BYTE f = new BLOCK8BYTE();
            internal BLOCK8BYTE X = new BLOCK8BYTE();

            internal BLOCK8BYTE DataBlockIn = new BLOCK8BYTE();
            internal BLOCK8BYTE DataBlockOut = new BLOCK8BYTE();
            internal BLOCK8BYTE DecryptXorBlock = new BLOCK8BYTE();

            /////////////////////////////////////////////////////////
            // Construction

            internal WORKING_SET()
            {

                // Build the arrays
                for (int i1 = 0; i1 < 17; i1++)
                {
                    Ln[i1] = new BLOCK8BYTE();
                    Rn[i1] = new BLOCK8BYTE();
                }

            }

            /////////////////////////////////////////////////////////
            // Operations

            internal void Scrub()
            {

                // Scrub data
                IP.Reset();
                for (int i1 = 0; i1 < 17; i1++)
                {
                    Ln[i1].Reset();
                    Rn[i1].Reset();
                }
                RnExpand.Reset();
                XorBlock.Reset();
                SBoxValues.Reset();
                f.Reset();
                X.Reset();
                DataBlockIn.Reset();
                DataBlockOut.Reset();
                DecryptXorBlock.Reset();

            }

        }

        /////////////////////////////////////////////////////////////
        // Constants

        public const int KEY_BYTE_LENGTH = 8;

        public const int BITS_PER_BYTE = 8;

        /////////////////////////////////////////////////////////////
        #region DES Tables

        /* PERMUTED CHOICE 1 (PCl) */
        private static byte[] bytePC1 = {	
			57,	49,	41,	33,	25,	17,	 9,
			1,	58,	50,	42,	34,	26,	18,
			10,	 2,	59,	51,	43,	35,	27,
			19,	11,	 3,	60,	52,	44,	36,
			63,	55,	47,	39,	31,	23,	15,
			7,	62,	54,	46,	38,	30,	22,
			14,	 6,	61,	53,	45,	37,	29,
			21,	13,	 5,	28,	20,	12,	 4,
		};

        /* PERMUTED CHOICE 2 (PC2) */
        private static byte[] bytePC2 = {	
			14,	17,	11,	24,	 1,	 5, 
			3,	28,	15,	 6,	21,	10, 
			23,	19,	12,	 4,	26,	 8, 
			16,	 7,	27,	20,	13,	 2, 
			41,	52,	31,	37,	47,	55, 
			30,	40,	51,	45,	33,	48, 
			44,	49,	39,	56,	34,	53, 
			46,	42,	50,	36,	29,	32, 
		};

        /* INITIAL PERMUTATION (IP) */
        private static byte[] byteIP =	{	
			58,	50,	42,	34,	26,	18,	10,	 2,	
			60,	52,	44,	36,	28,	20,	12,	 4,	
			62,	54,	46,	38,	30,	22,	14,	 6,	
			64,	56,	48,	40,	32,	24,	16,	 8,	
			57,	49,	41,	33,	25,	17,	 9,	 1,	
			59,	51,	43,	35,	27,	19,	11,	 3,	
			61,	53,	45,	37,	29,	21,	13,	 5,	
			63,	55,	47,	39,	31,	23,	15,	 7
		};

        /* REVERSE FINAL PERMUTATION (IP-1) */
        private static byte[] byteRFP =	{	 
			40,  8,   48,    16,    56,   24,    64,   32,
			39,  7,   47,    15,    55,   23,    63,   31,
			38,  6,   46,    14,    54,   22,    62,   30,
			37,  5,   45,    13,    53,   21,    61,   29,
			36,  4,   44,    12,    52,   20,    60,   28,
			35,  3,   43,    11,    51,   19,    59,   27,
			34,  2,   42,    10,    50,   18,    58,   26,
			33,  1,   41,     9,    49,   17,    57,   25,
		};

        /* E BIT-SELECTION TABLE */
        private static byte[] byteE = {	
			32,	 1,	 2,	 3,	 4,	 5,	
			4,	 5,	 6,	 7,	 8,	 9,	
			8,	 9,	10,	11,	12,	13,	
			12,	13,	14,	15,	16,	17,	
			16,	17,	18,	19,	20,	21,	
			20,	21,	22,	23,	24,	25,	
			24,	25,	26,	27,	28,	29,	
			28,	29,	30,	31,	32,	 1
		};

        /* PERMUTATION FUNCTION P */
        private static byte[] byteP = {	
			16,	 7,	20,	21,	
			29,	12,	28,	17,	
			1,	15,	23,	26,	
			5,	18,	31,	10,	
			2,	 8,	24,	14,	
			32,	27,	 3,	 9,	
			19,	13,	30,	 6,	
			22,	11,	 4,	25
		};

        // Schedule of left shifts for C and D blocks
        private static byte[] byteShifts = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        // S-Boxes
        private static byte[,] byteSBox = new byte[,] {
			{14,  4, 13,  1,	 2,	15,	11,	 8, 3, 10,  6, 12,	 5,	 9,	 0,	 7},
			{ 0, 15,  7,  4,	14,	 2,	13,	 1, 10,  6, 12, 11,	 9,	 5,	 3,	 8},
			{ 4,  1, 14,  8,	13,	 6,	 2,	11,	15, 12,  9,  7,	 3,	10,	 5,	 0},	
			{15, 12,  8,  2, 4,	 9,	 1,	 7,	  5, 11,  3, 14,	10,	 0,	 6,	13},
 
			{15, 1,	 8,	14,	 6,	11,	 3,	 4,	 9, 7,	 2,	13,	12,	 0,	 5,	10},	
			{3,	13,	 4,	 7,	15,	 2,	 8,	14,	12, 0,	 1,	10,	 6,	 9,	11,	 5},	
			{0,	14,	 7,	11,	10,	 4,	13,	 1,	5, 8,	12,	 6,	 9,	 3,	 2,	15},	
			{13, 8,	10,	 1,	 3,	15,	 4,	 2,	11, 6,	 7,	12,	 0,	 5,	14,	 9},
 
			{10,	 0,	 9,	14,	 6,	 3,	15,	 5,	 1,	13,	12,	 7,	11,	 4,	 2,	 8},	
			{13,	 7,	 0,	 9,	 3,	 4,	 6,	10,	 2,	 8,	 5,	14,	12,	11,	15,	 1},	
			{13,	 6,	 4,	 9,	 8,	15,	 3,	 0,	11,	 1,	 2,	12,	 5,	10,	14,	 7},	
			{1,	10,	13,	 0,	 6,	 9,	 8,	 7,	 4,	15,	14,	 3,	11,	 5,	 2,	12},	
 
			{7,	13,	14,	 3,	 0,	 6,	 9,	10,	 1,	 2,	 8,	 5,	11,	12,	 4,	15},	
			{13,	 8,	11,	 5,	 6,	15,	 0,	 3,	 4,	 7,	 2,	12,	 1,	10,	14,	 9},	
			{10,	 6,	 9,	 0,	12,	11,	 7,	13,	15,	 1,	 3,	14,	 5,	 2,	 8,	 4},	
			{3,	15,	 0,	 6,	10,	 1,	13,	 8,	9,	 4,	 5,	11,	12,	 7,	 2,	14},	
 
			{2,	12,	 4,	 1,	 7,	10,	11,	6,	8,	 5,	 3,	15,	13,	 0,	14,	 9},	
			{14,	11,	 2,	12,	 4,	 7,	13,	 1,	5,	 0,	15,	10,	 3,	 9,	 8,	 6},	
			{4,	 2,	 1,	11,	10,	13,	 7,	 8,15,	 9,	12,	 5,	 6,	 3,	 0,	14},	
			{11,	 8,	12,	 7,	 1,	14,	 2,	13,	6,	15,	 0,	 9,	10,	 4,	 5,	 3},	
 
			{12,	 1,	10,	15,	 9,	 2,	 6,	 8,0,	13,	 3,	 4,	14,	 7,	 5,	11},	
			{10,	15,	 4,	 2,	 7,	12,	 9,	 5,6,	 1,	13,	14,	 0,	11,	 3,	 8},	
			{9,	14,	15,	 5,	 2,	 8,	12,	 3,7,	 0,	 4,	10,	 1,	13,	11,	 6},	
			{4,	 3,	 2,	12,	 9,	 5,	15,	10,11,	14,	 1,	 7,	 6,	 0,	 8,	13},	
 
			{4,	11,	 2,	14,	15,	 0,	 8,	13,	3,	12,	 9,	 7,	 5,	10,	 6,	 1},	
			{13,	 0,	11,	 7,	 4,	 9,	 1,	10,14,	 3,	 5,	12,	 2,	15,	 8,	 6},	
			{1,	 4,	11,	13,	12,	 3,	 7,	14,10,	15,	 6,	 8,	 0,	 5,	 9,	 2},	
			{6,	11,	13,	 8,	 1,	 4,	10,	 7,9,	 5,	 0,	15,	14,	 2,	 3,	12},	
 
			{13,	 2,	 8,	 4,	 6,	15,	11,	 1,10,	 9,	 3,	14,	 5,	 0,	12,	 7},	
			{1,	15,	13,	 8,	10,	 3,	 7,	 4,	12,	 5,	 6,	11,	 0,	14,	 9,	 2},	
			{7,	11,	 4,	 1,	 9,	12,	14,	 2,0,	 6,	10,	13,	15,	 3,	 5,	 8},	
			{2,	 1,	14,	 7,	 4,	10,	 8,	13,15,	12,	 9,	 0,	 3,	 5,	 6,	11}
		};

        #endregion DES Tables

        /////////////////////////////////////////////////////////////
        #region Static Operations - DES

        public static bool IsValidDESKey(byte[] Key)
        {

            // Shortcuts
            if (Key == null)
                return false;
            if (Key.Length != KEY_BYTE_LENGTH)
                return false;
            if (!IsStrongDESKey(Key))
                return false;

            // Make sure end bits have odd parity
            for (int iByteOffset = 0; iByteOffset < KEY_BYTE_LENGTH; iByteOffset++)
            {

                // Add bits for this byte
                int iTotalBits = 0;
                byte Mask = 1;
                for (int iBitOffset = 0; iBitOffset < BITS_PER_BYTE; iBitOffset++)
                {
                    if ((Key[iByteOffset] & Mask) != 0)
                        iTotalBits++;
                    Mask <<= 1;
                }

                // If the total bits is not odd...
                if ((iTotalBits % 2) != 1)
                    return false;

            }

            // Return success
            return true;

        }

        public static bool IsStrongDESKey(byte[] Key)
        {

            // Compare by large integer
            UInt64 uiKey = BitConverter.ToUInt64(Key, 0);

            // Find weak keys...
            if (
                (uiKey == 0x0000000000000000) ||
                (uiKey == 0x00000000FFFFFFFF) ||
                (uiKey == 0xE0E0E0E0F1F1F1F1) ||
                (uiKey == 0x1F1F1F1F0E0E0E0E)
            )
                return false;

            // Find semi-weak keys...
            if (
                (uiKey == 0x011F011F010E010E) ||
                (uiKey == 0x1F011F010E010E01) ||
                (uiKey == 0x01E001E001F101F1) ||
                (uiKey == 0xE001E001F101F101) ||
                (uiKey == 0x01FE01FE01FE01FE) ||
                (uiKey == 0xFE01FE01FE01FE01) ||
                (uiKey == 0x1FE01FE00EF10EF1) ||
                (uiKey == 0xE01FE01FF10EF10E) ||
                (uiKey == 0x1FFE1FFE0EFE0EFE) ||
                (uiKey == 0xFE1FFE1FFE0EFE0E) ||
                (uiKey == 0xE0FEE0FEF1FEF1FE) ||
                (uiKey == 0xFEE0FEE0FEF1FEF1)
            )
                return false;

            // Return success
            return true;

        }

        /// <summary>
        /// I put this method because I need to implement padding mechanism that uses byte 0 as padding. 
        /// This is not available by default.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="PadLength"></param>
        /// <param name="Padding"></param>
        /// <returns></returns>
        public static byte[] DoPadWithString(byte[] Input, int PadLength, byte Padding)
        {
            int slen = Input.Length;
            int i = PadLength - (Input.Length % PadLength);

            //Debug.WriteLine("SLEN: " + slen);
            //Debug.WriteLine("I: " + i);
            //Debug.WriteLine("I: " + (i % PadLength));
            //Debug.WriteLine("PadLength: " + PadLength);

            if ((i > 0) && (i % PadLength < PadLength))
            {
                //Debug.WriteLine("Masuk IF, pad length: " + PadLength);
                byte[] res = new byte[((Input.Length / PadLength) + 1) * PadLength];

                Array.Copy(Input, 0, res, 0, slen);
                //slen = Input.Length % PadLength;

                //Debug.WriteLine("SLEN: " + slen);
                //for (i = Input.Length + slen; i < PadLength; i++)
                while (slen % PadLength > 0)
                {
                    //Debug.WriteLine("+ SLEN: " + slen);
                    res[slen] = Padding;
                    slen++;
                }
                //Debug.WriteLine("RES: " + res + "; LENGTH: " + res.Length);

                Input = res;
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                //Debug.WriteLine("Res: " + encoding.GetString(Input, 0, Input.Length));
                return Input;
            }
            else return Input;
        }

        public static void DES(byte[] bufferIn, ref byte[] bufferOut, byte[] Key, bool bEncrypt)
        {

            // Shortcuts
            if (!IsValidDESKey(Key))
                throw new Exception("Invalid DES key.");

            // Create the output buffer
            _createBufferOut(bufferIn.Length, ref bufferOut, bEncrypt);

            // Expand the keys into Kn
            KEY_SET[] Kn = new KEY_SET[1] {
				_expandKey(Key, 0)
			};

            // Apply DES keys
            _desAlgorithm(bufferIn, ref bufferOut, Kn, bEncrypt);

            // If decrypting...
            if (!bEncrypt)
                _removePadding(ref bufferOut);

        }

        #endregion Static Operations - DES

        /////////////////////////////////////////////////////////////
        #region Static Operations - TripleDES

        /// <summary>
        /// Sorry, I commented some lines to match my requirement. I don't need to check 
        /// wether there are any same sub keys or not.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static bool IsValidTripleDESKey(byte[] Key)
        {

            // Shortcuts
            if (Key == null)
                return false;
            if (Key.Length != (3 * KEY_BYTE_LENGTH))
                return false;

            // Check each part of the key
            /*
            byte[] SubKey = new byte[KEY_BYTE_LENGTH];
            for (int iKeyLoop = 0; iKeyLoop < 3; iKeyLoop++)
            {
 
                // Get sub-key
                Array.Copy(Key, iKeyLoop * 8, SubKey, 0, KEY_BYTE_LENGTH);
 
                // Check this DES key
                if (!IsValidDESKey(SubKey))
                    return false;
 
            }
 
            // Keys must not be equal
            /*
            bool bAEqualsB = true;
            bool bAEqualsC = true;
            bool bBEqualsC = true;
            for (int iByteOffset = 0; iByteOffset < KEY_BYTE_LENGTH; iByteOffset++)
            {
                if (Key[iByteOffset] != Key[iByteOffset + KEY_BYTE_LENGTH])
                    bAEqualsB = false;
                if (Key[iByteOffset] != Key[iByteOffset + KEY_BYTE_LENGTH + KEY_BYTE_LENGTH])
                    bAEqualsC = false;
                if (Key[iByteOffset + KEY_BYTE_LENGTH] != Key[iByteOffset + KEY_BYTE_LENGTH + KEY_BYTE_LENGTH])
                    bBEqualsC = false;
            }
            if ((bAEqualsB) || (bAEqualsC) || (bBEqualsC))
                return false;
            */

            // Return success
            return true;

        }

        /// <summary>
        /// Sorry, I need to comment _removePadding because I implement my own padding mechanism. So I don't need the original remove padding.
        /// </summary>
        /// <param name="bufferIn"></param>
        /// <param name="bufferOut"></param>
        /// <param name="Key"></param>
        /// <param name="bEncrypt"></param>
        public static void TripleDES(byte[] bufferIn, ref byte[] bufferOut, byte[] Key, bool bEncrypt)
        {

            // Shortcuts
            if (!IsValidTripleDESKey(Key))
                throw new Exception("Invalid DES key.");

            // Create the output buffer
            _createBufferOut(bufferIn.Length, ref bufferOut, bEncrypt);

            // Expand the keys into Kn
            KEY_SET[] Kn = new KEY_SET[3] {
				_expandKey(Key, 0),
				_expandKey(Key, 8),
				_expandKey(Key, 16)
			};

            // Apply DES keys
            _desAlgorithm(bufferIn, ref bufferOut, Kn, bEncrypt);

            // If decrypting...
            //if (!bEncrypt)
            //    _removePadding(ref bufferOut);

        }

        #endregion Static Operations - TripleDES

        /////////////////////////////////////////////////////////////
        #region Static Operations

        private static void _incKey(byte[] Key, int iInc)
        {
#if DEBUG
            Debug.Assert(Key.Length == KEY_BYTE_LENGTH);
#endif // #if DEBUG

            // shortcuts
            if (iInc == 0)
                return;

            // Add the increment				
            int iCarry = iInc;
            for (int iByteOffset = 0; iByteOffset < KEY_BYTE_LENGTH; iByteOffset++)
            {
                int iTemp = Key[iByteOffset] + iCarry;
                iCarry = iTemp >> 8;
                Key[iByteOffset] = Convert.ToByte(iTemp & 0xFF);
                if (iCarry == 0)
                    break;
            }

        }

        private static void _modifyKeyParity(byte[] Key)
        {
#if DEBUG
            Debug.Assert(Key.Length == KEY_BYTE_LENGTH);
#endif // #if DEBUG

            // Make sure end bits have odd parity
            for (int iByteOffset = 0; iByteOffset < KEY_BYTE_LENGTH; iByteOffset++)
            {

                // Add bits for this byte
                int iTotalBits = 0;
                byte Mask = 1;
                for (int iBitOffset = 0; iBitOffset < BITS_PER_BYTE; iBitOffset++)
                {
                    if ((Key[iByteOffset] & Mask) != 0)
                        iTotalBits++;
                    Mask <<= 1;
                }

                // If the total bits is not odd...
                if ((iTotalBits % 2) != 1)
                {

                    // Flip the first bit to retain odd parity
                    Key[iByteOffset] ^= 0x01;

                }

            }

        }

        private static KEY_SET _expandKey(byte[] Key, int iOffset)
        {

            //
            // Expand an 8 byte DES key into a set of permuted keys
            //

            // Declare return variable
            KEY_SET Ftmp = new KEY_SET();

            // Declaration of local variables
            int iTableOffset, iArrayOffset, iPermOffset, iByteOffset, iBitOffset;
            bool bBit;

            // Put key into an 8-bit block
            BLOCK8BYTE K = new BLOCK8BYTE();
            K.Set(Key, iOffset);

            // Permutate Kp with PC1
            BLOCK8BYTE Kp = new BLOCK8BYTE();
            for (iArrayOffset = 0; iArrayOffset < bytePC1.Length; iArrayOffset++)
            {

                // Get permute offset
                iPermOffset = bytePC1[iArrayOffset];
                iPermOffset--;

                // Get and set bit
                Kp.SetBit(
                    _bitAddressToByteOffset(iArrayOffset, 7),
                    _bitAddressToBitOffset(iArrayOffset, 7),
                    K.GetBit(
                        _bitAddressToByteOffset(iPermOffset, 8),
                        _bitAddressToBitOffset(iPermOffset, 8)
                    )
                );

            }

            // Create 17 blocks of C and D from Kp
            BLOCK8BYTE[] KpCn = new BLOCK8BYTE[17];
            BLOCK8BYTE[] KpDn = new BLOCK8BYTE[17];
            for (iArrayOffset = 0; iArrayOffset < 17; iArrayOffset++)
            {
                KpCn[iArrayOffset] = new BLOCK8BYTE();
                KpDn[iArrayOffset] = new BLOCK8BYTE();
            }
            for (iArrayOffset = 0; iArrayOffset < 32; iArrayOffset++)
            {

                // Set bit in KpCn
                iByteOffset = _bitAddressToByteOffset(iArrayOffset, 8);
                iBitOffset = _bitAddressToBitOffset(iArrayOffset, 8);
                bBit = Kp.GetBit(iByteOffset, iBitOffset);
                KpCn[0].SetBit(iByteOffset, iBitOffset, bBit);

                // Set bit in KpDn
                bBit = Kp.GetBit(iByteOffset + 4, iBitOffset);
                KpDn[0].SetBit(iByteOffset, iBitOffset, bBit);

            }
            for (iArrayOffset = 1; iArrayOffset < 17; iArrayOffset++)
            {

                // Shift left wrapped
                KpCn[iArrayOffset].ShiftLeftWrapped(KpCn[iArrayOffset - 1], byteShifts[iArrayOffset - 1]);
                KpDn[iArrayOffset].ShiftLeftWrapped(KpDn[iArrayOffset - 1], byteShifts[iArrayOffset - 1]);

            }

            // Create 17 keys Kn
            for (iArrayOffset = 0; iArrayOffset < 17; iArrayOffset++)
            {

                // Loop through the bits
                for (iTableOffset = 0; iTableOffset < 48; iTableOffset++)
                {

                    // Get address if bit
                    iPermOffset = bytePC2[iTableOffset];
                    iPermOffset--;

                    // Convert to byte and bit offsets
                    iByteOffset = _bitAddressToByteOffset(iPermOffset, 7);
                    iBitOffset = _bitAddressToBitOffset(iPermOffset, 7);

                    // Get bit
                    if (iByteOffset < 4)
                        bBit = KpCn[iArrayOffset].GetBit(iByteOffset, iBitOffset);
                    else
                        bBit = KpDn[iArrayOffset].GetBit(iByteOffset - 4, iBitOffset);

                    // Set bit
                    iByteOffset = _bitAddressToByteOffset(iTableOffset, 6);
                    iBitOffset = _bitAddressToBitOffset(iTableOffset, 6);
                    Ftmp.GetAt(iArrayOffset).SetBit(iByteOffset, iBitOffset, bBit);

                }

            }

            // Return variable
            return Ftmp;

        }

        private static void _createBufferOut(int iBufferInLength, ref byte[] bufferOut, bool bEncrypt)
        {

            //
            // Create a buffer for the output, which may be trimmed later
            //

            // If encrypting...
            int iOutputLength;
            if (bEncrypt)
            {
                if ((iBufferInLength % KEY_BYTE_LENGTH) != 0)
                {
                    iOutputLength = ((iBufferInLength / KEY_BYTE_LENGTH) + 1) * KEY_BYTE_LENGTH;
                    //iOutputLength = ((iBufferInLength / KEY_BYTE_LENGTH)) * KEY_BYTE_LENGTH;
                }
                else
                {
                    //iOutputLength = iBufferInLength + KEY_BYTE_LENGTH;
                    iOutputLength = iBufferInLength;
                }
            }
            else
            {
                if (iBufferInLength < 8)
                    throw new Exception("DES cypher-text must be at least 8 bytes.");
                if ((iBufferInLength % 8) != 0)
                    throw new Exception("DES cypher-text must be a factor of 8 bytes in length.");
                iOutputLength = iBufferInLength;
            }

            // Create buffer
            if ((bufferOut == null) || (bufferOut.Length != iOutputLength))
                bufferOut = new byte[iOutputLength];
            else
                Array.Clear(bufferOut, 0, bufferOut.Length);

        }

        private static void _removePadding(ref byte[] bufferOut)
        {

            //
            // Remove the padding after decrypting
            //

            // Get the padding...
            byte Padding = bufferOut[bufferOut.Length - 1];
            if ((Padding == 0) || (Padding > 8))
                throw new Exception("Invalid padding on DES data.");

            // Confirm padding
            bool bPaddingOk = true;
            for (int iByteOffset = 1; iByteOffset < Padding; iByteOffset++)
            {
                if (bufferOut[bufferOut.Length - 1 - iByteOffset] != Padding)
                {
                    bPaddingOk = false;
                    break;
                }
            }
            if (bPaddingOk)
            {

                // Chop off the padding
                Array.Resize(ref bufferOut, bufferOut.Length - Padding);

            }
            else
                throw new Exception("Invalid padding on DES data.");

        }

        private static void _desAlgorithm(byte[] bufferIn, ref byte[] bufferOut, KEY_SET[] KeySets, bool bEncrypt)
        {

            //
            // Apply the DES algorithm to each block
            //

            // Declare a workset set of variables
            WORKING_SET workingSet = new WORKING_SET();

            // encode/decode blocks
            int iBufferPos = 0;
            while (true)
            {

                // Check buffer position
                if (bEncrypt)
                {

                    // If end of buffer...
                    if (iBufferPos >= bufferOut.Length)
                        break;

                    // Calulate remaining bytes
                    int iRemainder = (bufferIn.Length - iBufferPos);
                    if (iRemainder >= 8)
                        workingSet.DataBlockIn.Set(bufferIn, iBufferPos);
                    else
                    {

                        // Copy part-block
                        workingSet.DataBlockIn.Reset();
                        if (iRemainder > 0)
                            Array.Copy(bufferIn, iBufferPos, workingSet.DataBlockIn.m_data, 0, iRemainder);

                        // Get the padding byte
                        byte Padding = Convert.ToByte(KEY_BYTE_LENGTH - iRemainder);

                        // Add padding to block
                        for (int iByteOffset = iRemainder; iByteOffset < KEY_BYTE_LENGTH; iByteOffset++)
                            workingSet.DataBlockIn.m_data[iByteOffset] = Padding;

                    }

                }
                else
                {

                    // If end of buffer...
                    if (iBufferPos >= bufferIn.Length)
                        break;

                    // Get the next block
                    workingSet.DataBlockIn.Set(bufferIn, iBufferPos);

                }

                // if encrypting and not the first block...
                if ((bEncrypt) && (iBufferPos > 0))
                {
                    // Amri commented these lines since Amri need EBC, not CBC
                    // Apply succession => XOR M with previous block
                    //workingSet.DataBlockIn.Xor(workingSet.DataBlockOut, workingSet.DataBlockIn);

                }

                // Apply the algorithm
                workingSet.DataBlockOut.Set(workingSet.DataBlockIn);
                _lowLevel_desAlgorithm(workingSet, KeySets, bEncrypt);

                // If decrypting...
                if (!bEncrypt)
                {

                    // Amri commented these lines since Amri need EBC, not CBC
                    // Retain the succession
                    //if (iBufferPos > 0)
                    //    workingSet.DataBlockOut.Xor(workingSet.DecryptXorBlock, workingSet.DataBlockOut);

                    // Retain the last block
                    workingSet.DecryptXorBlock.Set(workingSet.DataBlockIn);

                }

                // Update buffer out
                Array.Copy(workingSet.DataBlockOut.m_data, 0, bufferOut, iBufferPos, 8);

                // Move on
                iBufferPos += 8;

            }

            // Scrub the working set
            workingSet.Scrub();

        }

        private static void _lowLevel_desAlgorithm(WORKING_SET workingSet, KEY_SET[] KeySets, bool bEncrypt)
        {

            //
            // Apply 1 or 3 keys to a block of data
            //

            // Declaration of local variables
            int iTableOffset;
            int iArrayOffset;
            int iPermOffset;
            int iByteOffset;
            int iBitOffset;

            // Loop through keys
            for (int iKeySetOffset = 0; iKeySetOffset < KeySets.Length; iKeySetOffset++)
            {

                // Permute with byteIP
                workingSet.IP.Reset();
                for (iTableOffset = 0; iTableOffset < byteIP.Length; iTableOffset++)
                {

                    // Get perm offset
                    iPermOffset = byteIP[iTableOffset];
                    iPermOffset--;

                    // Get and set bit
                    workingSet.IP.SetBit(
                        _bitAddressToByteOffset(iTableOffset, 8),
                        _bitAddressToBitOffset(iTableOffset, 8),
                        workingSet.DataBlockOut.GetBit(
                            _bitAddressToByteOffset(iPermOffset, 8),
                            _bitAddressToBitOffset(iPermOffset, 8)
                        )
                    );

                }

                // Create Ln[0] and Rn[0]
                workingSet.Ln[0].Reset();
                workingSet.Rn[0].Reset();
                for (iArrayOffset = 0; iArrayOffset < 32; iArrayOffset++)
                {
                    iByteOffset = _bitAddressToByteOffset(iArrayOffset, 8);
                    iBitOffset = _bitAddressToBitOffset(iArrayOffset, 8);
                    workingSet.Ln[0].SetBit(iByteOffset, iBitOffset, workingSet.IP.GetBit(iByteOffset, iBitOffset));
                    workingSet.Rn[0].SetBit(iByteOffset, iBitOffset, workingSet.IP.GetBit(iByteOffset + 4, iBitOffset));
                }

                // Loop through 17 interations
                for (int iBlockOffset = 1; iBlockOffset < 17; iBlockOffset++)
                {

                    // Get the array offset
                    int iKeyOffset;
                    if (bEncrypt != (iKeySetOffset == 1))
                        iKeyOffset = iBlockOffset;
                    else
                        iKeyOffset = 17 - iBlockOffset;

                    // Set Ln[N] = Rn[N-1]
                    workingSet.Ln[iBlockOffset].Set(workingSet.Rn[iBlockOffset - 1]);

                    // Set Rn[N] = Ln[0] + f(R[N-1],K[N])
                    for (iTableOffset = 0; iTableOffset < byteE.Length; iTableOffset++)
                    {

                        // Get perm offset
                        iPermOffset = byteE[iTableOffset];
                        iPermOffset--;

                        // Get and set bit
                        workingSet.RnExpand.SetBit(
                            _bitAddressToByteOffset(iTableOffset, 6),
                            _bitAddressToBitOffset(iTableOffset, 6),
                            workingSet.Rn[iBlockOffset - 1].GetBit(
                                _bitAddressToByteOffset(iPermOffset, 8),
                                _bitAddressToBitOffset(iPermOffset, 8)
                            )
                        );

                    }

                    // XOR expanded block with K-block
                    if (bEncrypt != (iKeySetOffset == 1))
                        workingSet.XorBlock.Xor(workingSet.RnExpand, KeySets[iKeySetOffset].GetAt(iKeyOffset));
                    else
                        workingSet.XorBlock.Xor(workingSet.RnExpand, KeySets[KeySets.Length - 1 - iKeySetOffset].GetAt(iKeyOffset));

                    // Set S-Box values
                    workingSet.SBoxValues.Reset();
                    for (iTableOffset = 0; iTableOffset < 8; iTableOffset++)
                    {

                        // Calculate m and n
                        int m = ((workingSet.XorBlock.GetBit(iTableOffset, 7) ? 1 : 0) << 1) | (workingSet.XorBlock.GetBit(iTableOffset, 2) ? 1 : 0);
                        int n = (workingSet.XorBlock.m_data[iTableOffset] >> 3) & 0x0F;

                        // Get s-box value
                        iPermOffset = byteSBox[(iTableOffset * 4) + m, n];
                        workingSet.SBoxValues.m_data[iTableOffset] = (byte)(iPermOffset << 4);

                    }

                    // Permute with P -> f
                    workingSet.f.Reset();
                    for (iTableOffset = 0; iTableOffset < byteP.Length; iTableOffset++)
                    {

                        // Get perm offset
                        iPermOffset = byteP[iTableOffset];
                        iPermOffset--;

                        // Get and set bit
                        workingSet.f.SetBit(
                            _bitAddressToByteOffset(iTableOffset, 4),
                            _bitAddressToBitOffset(iTableOffset, 4),
                            workingSet.SBoxValues.GetBit(
                                _bitAddressToByteOffset(iPermOffset, 4),
                                _bitAddressToBitOffset(iPermOffset, 4)
                            )
                        );

                    }

                    // Rn[N] = Ln[N-1] ^ f
                    workingSet.Rn[iBlockOffset].Reset();
                    for (iTableOffset = 0; iTableOffset < 8; iTableOffset++)
                    {

                        // Get Ln[N-1] -> A
                        byte A = workingSet.Ln[iBlockOffset - 1].m_data[(iTableOffset >> 1)];
                        if ((iTableOffset % 2) == 0)
                            A >>= 4;
                        else
                            A &= 0x0F;

                        // Get f -> B
                        byte B = Convert.ToByte(workingSet.f.m_data[iTableOffset] >> 4);

                        // Update Rn[N]
                        if ((iTableOffset % 2) == 0)
                            workingSet.Rn[iBlockOffset].m_data[iTableOffset >> 1] |= Convert.ToByte((A ^ B) << 4);
                        else
                            workingSet.Rn[iBlockOffset].m_data[iTableOffset >> 1] |= Convert.ToByte(A ^ B);

                    }

                }

                // X = R16 L16
                workingSet.X.Reset();
                for (iTableOffset = 0; iTableOffset < 4; iTableOffset++)
                {
                    workingSet.X.m_data[iTableOffset] = workingSet.Rn[16].m_data[iTableOffset];
                    workingSet.X.m_data[iTableOffset + 4] = workingSet.Ln[16].m_data[iTableOffset];
                }

                // C = X perm IP
                workingSet.DataBlockOut.Reset();
                for (iTableOffset = 0; iTableOffset < byteRFP.Length; iTableOffset++)
                {

                    // Get perm offset
                    iPermOffset = byteRFP[iTableOffset];
                    iPermOffset--;

                    // Get and set bit
                    workingSet.DataBlockOut.SetBit(
                        _bitAddressToByteOffset(iTableOffset, 8),
                        _bitAddressToBitOffset(iTableOffset, 8),
                        workingSet.X.GetBit(
                            _bitAddressToByteOffset(iPermOffset, 8),
                            _bitAddressToBitOffset(iPermOffset, 8)
                        )
                    );

                }

            } // key loop

        }

        #endregion Static Operations

        /////////////////////////////////////////////////////////////
        // Helper Operations

        private static int _bitAddressToByteOffset(int iTableAddress, int iTableWidth)
        {
            int iFtmp = iTableAddress / iTableWidth;
            return iFtmp;
        }

        private static int _bitAddressToBitOffset(int iTableAddress, int iTableWidth)
        {
            int iFtmp = BITS_PER_BYTE - 1 - (iTableAddress % iTableWidth);
            return iFtmp;
        }

        /////////////////////////////////////////////////////////////
        #region Debug Operations

#if DEBUG
#if !SILVERLIGHT
		private static void MicrosoftDESEncrypt(byte[] bufferIn, ref byte[] bufferOut, byte[] Key, bool bEncrypt, bool bDESMode)
		{
 
			// Declaration of key and IV
			byte[] bufferTemp = new byte[1024];
			byte[] IV;
			if(bDESMode)
				IV = new byte[8];
			else
				IV = new byte[8*3];
 
			// Declare a crypto object
			ICryptoTransform crypto;
			if (bDESMode)
			{
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				des.Padding = PaddingMode.PKCS7;
				if (bEncrypt)
					crypto = des.CreateEncryptor(Key, IV);
				else
					crypto = des.CreateDecryptor(Key, IV);
			}
			else
			{
				TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider();
				tripleDes.Padding = PaddingMode.PKCS7;
				if (bEncrypt)
					crypto = tripleDes.CreateEncryptor(Key, IV);
				else
					crypto = tripleDes.CreateDecryptor(Key, IV);
			}			
 
			//  a memory stream for the cyrpto
			using(MemoryStream ms = new MemoryStream())
			{
 
				// Create a CryptoStream using the memory stream
				using (CryptoStream encStream = new CryptoStream(ms, crypto, CryptoStreamMode.Write))
				{
 
					// Encrypt/decrypt and flush
					encStream.Write(bufferIn, 0, bufferIn.Length);
					encStream.Flush();
					encStream.FlushFinalBlock();
					encStream.Close();
 
					// Get the data into a buffer
					bufferOut = ms.ToArray();
 
				}
 
			}
 
		}
#endif // #if !SILVERLIGHT
#endif // #if DEBUG

#if DEBUG
#if !SILVERLIGHT
		public static void _assertBufferMatch(byte[] A, byte[] B)
		{
 
			// Compare outputs
			Debug.Assert(A.Length == B.Length);
			for (int iOffset = 0; iOffset < A.Length; iOffset++)
				Debug.Assert(A[iOffset] == B[iOffset]);
 
		}
#endif // #if !SILVERLIGHT
#endif // #if DEBUG

#if DEBUG
#if !SILVERLIGHT
		public static void Test()
		{
 
			//
			// This function encrypts and encrypts data using our DES algorithm, and 
			// ensures the results are the same as the Microsoft algorithm with default padding settings.
			//
 
			// Declaration of local variables
			Random rnd = new Random(1);
			byte[] DesKey, Des3Key;
			byte[] plainText = null, cypherText = null, plainText2 = null, msCypherText = null, msPlainText2 = null;
 
			// Compare the DES algorithm with the Microsoft algorithm
			for (int iTest = 0; iTest < 100*1000; iTest++)
			{
 
				// Dump progress
				if ((iTest % 200) == 0)
					Trace.TraceInformation("Test {0}", iTest);
 
				// Generate test data
				DesKey = DESCrytography.CreateDesKey(rnd);
				Des3Key = DESCrytography.CreateTripleDesKey(rnd);
				int iLength = rnd.Next(0, 256);
				if ((plainText == null) || (plainText.Length != iLength))
					plainText = new byte[iLength];
				rnd.NextBytes(plainText);
 
				// DES Test
				{
 
					// Encrypt using our algorithm
					DESCrytography.DES(plainText, ref cypherText, DesKey, true);
 
					// Decrypt using our algorithm
					DESCrytography.DES(cypherText, ref plainText2, DesKey, false);
 
					// Compare outputs
					_assertBufferMatch(plainText,plainText2);
 
					// Encrypt using Microsoft algorithm
					MicrosoftDESEncrypt(plainText, ref msCypherText, DesKey, true, true);
 
					// Decrypt using Microsoft algorithm
					MicrosoftDESEncrypt(msCypherText, ref msPlainText2, DesKey, false, true);
 
					// Compare outputs
					_assertBufferMatch(plainText, msPlainText2);
 
					// Make sure Microsoft and our algorithms are the same
					_assertBufferMatch(cypherText, msCypherText);
 
				}
 
				// TripleDES Test
				{
					
					// Encrypt using our algorithm
					DESCrytography.TripleDES(plainText, ref cypherText, Des3Key, true);
 
					// Decrypt using our algorithm
					DESCrytography.TripleDES(cypherText, ref plainText2, Des3Key, false);
 
					// Compare outputs
					_assertBufferMatch(plainText, plainText2);
 
					// Encrypt using Microsoft algorithm
					MicrosoftDESEncrypt(plainText, ref msCypherText, Des3Key, true, false);
 
					// Decrypt using Microsoft algorithm
					MicrosoftDESEncrypt(msCypherText, ref msPlainText2, Des3Key, false, false);
 
					// Compare outputs
					_assertBufferMatch(plainText, msPlainText2);
 
					// Make sure Microsoft and our algorithms are the same
					_assertBufferMatch(cypherText, msCypherText);
 
				}
 
			} // for-loop
 
		}
#endif // #if !SILVERLIGHT
#endif // #if DEBUG

        #endregion Debug Operations

    }

}