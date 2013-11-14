//  This code is for demonstration only and has not been heavily tested or optimized.
//
//	This is a simple fixed function shader
//  It will not work with Dx11 9.1 (Shader model 2)
//
//  This can be demonstrated within the unity editor by setting
//  (main menu)->edit->graphics emulation to "Dx11 9.1 (Shader model 2) No Fixed Function"
//

Shader "Custom/FixedFunctionShader" {
 Properties {
  _MainTex ("Base (RGB)", 2D) = "white" {}
 }
 SubShader {
  Tags {"Queue"="Transparent" }
  Blend DstColor Zero
  Lighting Off
  Zwrite Off
  ZTest Always
  Pass { 
   SetTexture[_MainTex]
  }
 }  
}