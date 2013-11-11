#if UNITY_METRO && NETFX_CORE && !UNITY_EDITOR

using System.Text.RegularExpressions;

namespace System.Net {
	public class IPAddress {
		
		public long Address { get; set; }
		
		public IPAddress(long newAddress) {
			if(newAddress < 0 || newAddress > 0x00000000FFFFFFFF)
				throw new ArgumentOutOfRangeException("Invalid IP address");
			Address = newAddress;			
		}
				
		public static bool TryParse(string ipString, out IPAddress address) {
			try {
				address = new IPAddress(IP2Long(ipString));
			} catch(Exception e)
			{	
				if(e is ArgumentNullException || e is ArgumentOutOfRangeException ||
					e is FormatException)
				{
					address = null;
					return false;
				}
				throw;
			}
			return true;
		}
		
		/**
		 * From http://geekswithblogs.net/rgupta/archive/2009/04/29/convert-ip-to-long-and-vice-versa-c.aspx
		 * and
		 * http://stackoverflow.com/questions/7770358/checking-that-a-user-has-correctly-entered-an-ip-address
		 */
		private static long IP2Long(string ipString) {
		    string[] ipBytes;
		    double num = 0;
			if (!Regex.IsMatch (ipString, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"))
                throw new FormatException("ipString is not a valid IP address");
		    if(string.IsNullOrEmpty(ipString))
				throw new ArgumentNullException("ipString is null or empty");
	    
	        ipBytes = ipString.Split('.');
	        for (int i = ipBytes.Length - 1; i >= 0; i--)
	        {
	            num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
	        }		    
				
		    return (long)num;
		}
	}
}
#endif