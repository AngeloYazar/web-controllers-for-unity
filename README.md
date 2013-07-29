Web Controllers for Unity
=============

This in an experiment I cobbled together to see if it was possible to connect a phone to Unity to be used as a controller without downloading an app.

It accomplishes this by directing the phone to a website with a QRCode.
The local IP of the computer running the game is included in the URL, so the controller can connect over the local network with a websocket.

The input lag is slightly too high for use in an action game, but it does demontrate that this is possible.
I have not yet tried it on an OUYA, only in Unity.

The goal is to enable OUYA games to support a large number of players, without too much headache for the players.

Currently it only supports mobile safari, version 6.0 and above. This is because the websocket server it uses only supports [RFC 6455](http://tools.ietf.org/html/rfc6455)

Resources used in the project:
- For Websockets: https://github.com/sta/websocket-sharp
- For QRCodes: http://zxingnet.codeplex.com/
- For the JS/HTML example: https://github.com/lipp/lua-websockets
- Images for the OUYA buttons from the docs: https://devs.ouya.tv/developers/docs/interface-guidelines


![alt text](http://www.yazarmediagroup.com/ouya/images/web-controllers-for-unity.png "Screenshot")
In this screenshot, the colored cubes are the 'players' displaying the last input they got from the controller.
