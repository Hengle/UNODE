﻿using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using WebSocketSharp;
using MiniMessagePack;

using Debug = UnityEngine.Debug;

public class Unode_v1_1 : MonoBehaviour {
	
	//Websocket
	public string adress;
	public WebSocket ws;
	public bool run;

	//Messagepack
	MiniMessagePacker pakage;
	private Dictionary<string,object>	Msgpack,
										packed_data,
										localPosition,
										localEulerAngles,
										localScale;
	private object obj;
	public string mode;

	//Node.exe
	public string program_name;
	public bool windowtype_hidden = false;
	public bool x86;

	[HideInInspector]
	public string nodejs_path;
	public string Script_path;

	public bool connected = false;
	private Process nodejs_process;
	private ProcessStartInfo info;
	public bool cmd = true;
	public string command;
	public string js;
	
	void Awake() {
		if(run = open_nodejs(x86))
			ws = new WebSocket(adress);
		 	pakage = new MiniMessagePacker ();
	}

	void Start () {
		if(run){
			ws.Connect ();
			//ws.WaitTime = TimeSpan.FromSeconds(10);
			var packed_data = new Dictionary<string, object> {
				{ "mode", "connect" },
				{ "name", name },
			};
			send(ws,packed_data);
		}

		ws.OnOpen += (sender, e) => {
			Debug.Log ("ws.OnOpen:");
		};
		
		ws.OnMessage += (sender, e) => {
			Msgpack = decode(e.RawData) as Dictionary<string,object>;
			mode = (string)Msgpack["mode"];
			switch(mode){
				case "connected":
					Debug.Log ((string)Msgpack["ver"]);
					connected = true;
					break;
				case "echo":
					Debug.Log ("echo:"+(string)Msgpack["text"]);
					break;
			}
		};

		ws.OnError += (object sender, ErrorEventArgs e) => {
			Debug.Log ("OnError[unode]:" + e.Message);
		};

		ws.OnClose += (object sender, CloseEventArgs e) => {
			Debug.Log ("OnClosed[unode]:" + e.Reason);
		};
	}
	
	//メインスレッド
	void Update () {
		if (run && ws.IsAlive && ws != null) {
			//Debug.Log("Ping:"+ws.Ping());
		} else {
			Debug.Log("Error:Not Run Node.js");
		}
	}

	void OnApplicationQuit() {
		if (ws != null) {
			ws.Close ();
		}
		kill_nodejs ();
	}
	
	private bool open_nodejs(bool arch){
		if (arch == true) {
			nodejs_path = Application.streamingAssetsPath + "/.node/x86/";
		} else {
			nodejs_path = Application.streamingAssetsPath + "/.node/x64/";
		}
			Script_path = Application.streamingAssetsPath + "/.node/src/";

		info = new System.Diagnostics.ProcessStartInfo();
		info.FileName = nodejs_path + program_name;
		info.WorkingDirectory = Script_path;

		if(cmd){
			info.Arguments = command;
		}else{
			info.Arguments = Script_path + js;
		}

		if(windowtype_hidden){
			info.WindowStyle = ProcessWindowStyle.Hidden;
		}

		try{
			nodejs_process = Process.Start(info);
		}catch(System.ComponentModel.Win32Exception w){
			Debug.Log("Not Found." + w);
			return false;
		}
		return true;
	}

	private void kill_nodejs(){
		if (!nodejs_process.HasExited) {
			nodejs_process.Kill ();
		}
	}

	public byte[] encode(Dictionary<string,object> data){
		return pakage.Pack(data);
	}

	public object decode(byte[] data){
		return pakage.Unpack(data);
	}

	public void send(WebSocket s,Dictionary<string,object> data){
		s.Send(encode(data));
	}

	public void regist_js(WebSocket s,string name,string js){
		packed_data = new Dictionary<string, object> {
			{ "mode", "child" },
			{ "regist", true},
			{ "name", name},
			{ "js", js}
		};
		send(s,packed_data);		
	}

	public void run_js(WebSocket s,string name,Dictionary<string,object> option){
		var packed_data = new Dictionary<string, object> {
			{ "mode", "child" },
			{ "name", name},
			{ "options", option}
		};
		send(s,packed_data);		
	}

	public void send_transform(WebSocket s,GameObject obj){
		localPosition = new Dictionary<string, object> {
			{ "x", obj.transform.localPosition.x},
			{ "y", obj.transform.localPosition.y},
			{ "z", obj.transform.localPosition.z}
		};
		localEulerAngles = new Dictionary<string, object> {
			{ "x", obj.transform.localEulerAngles.x},
			{ "y", obj.transform.localEulerAngles.y},
			{ "z", obj.transform.localEulerAngles.z}
		};
		localScale = new Dictionary<string, object> {
			{ "x", obj.transform.localScale.x},
			{ "y", obj.transform.localScale.y},
			{ "z", obj.transform.localScale.z}
		};
		packed_data = new Dictionary<string, object> {
			{ "mode", "transform" },
			{ "name", obj.name},
			{ "localPosition", localPosition},
			{ "localEulerAngles", localEulerAngles},
			{ "localScale", localScale},
		};
		ws.Send("atestsaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
		//ws.SendAsync ("test");
	}

	public void recive_transform(Dictionary<string,object> transformData){
		var l_pos         = (Dictionary<string,object>)transformData["localPosition"];
		var l_EulerAngles = (Dictionary<string,object>)transformData["localPosition"];
		var l_Scale       = (Dictionary<string,object>)transformData["localPosition"];
		
		transform.localPosition    = new Vector3((long)l_pos["x"],(long)l_pos["y"],(long)l_pos["z"]);
		transform.localEulerAngles = new Vector3((long)l_EulerAngles["x"],(long)l_EulerAngles["y"],(long)l_EulerAngles["z"]);
		transform.localScale       = new Vector3((long)l_Scale["x"],(long)l_Scale["y"],(long)l_Scale["z"]);
	}
}
