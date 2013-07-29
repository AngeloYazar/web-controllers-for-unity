using UnityEngine;
using System.Collections;
using ZXing.Common;

public class QRCode : MonoBehaviour {
	
	private ZXing.QrCode.QRCodeWriter QRWriter;
	
	void Start () {
		string url = "http://www.yazarmediagroup.com/ouya/index.html?" + Network.player.ipAddress.ToString();
		QRWriter = new ZXing.QrCode.QRCodeWriter();
		BitMatrix encoded = QRWriter.encode(url, ZXing.BarcodeFormat.QR_CODE, 512, 512);
		Texture2D tex = new Texture2D(512,512,TextureFormat.RGBA32,false);
		Color[] pixels = tex.GetPixels();
		int k = 0;
		for(int j = 0; j < 512; j++ ) {
			ZXing.Common.BitArray row = new ZXing.Common.BitArray(512);
			row = encoded.getRow(j,null);
			int[] intRow = row.Array;
			for( int i = intRow.Length-1; i >= 0; i-- ) {
				int thirtyTwoPixels = intRow[i];
				for(int b = 31; b >= 0; b--) {
					int pixel = ( (thirtyTwoPixels >> b) & 1 );
					if( pixel == 0 ) {
						pixels[k] = Color.white;
					}
					else {
						pixels[k] = Color.black;
					}
					k++;
				}
			}
		}
		tex.SetPixels( pixels );
		tex.Apply();
		renderer.material.mainTexture = tex;
	}
}
