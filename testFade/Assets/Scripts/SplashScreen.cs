using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	
	//要加载的关卡
	public string LevelToLoad="DiveUnityDemo";
	//Logo贴图
	public Texture2D SplashLogo;
	//渐入渐出速度
	public float FadeSpeed=0.3F;
	//等待时间
	public float WaitTime=0.5F;
	
	#region 渐入渐出的类型
	public enum SplashType
	{
		LoadLevelThenFadeOut,
		FadeOutThenLoadLevel
	}
	public SplashType Type=SplashType.LoadLevelThenFadeOut;
	#endregion
	
	#region 渐入渐出的状态
	public enum FadeStatus
	{
		FadeIn,
		FadeWait,
		FadeOut
	}
	private FadeStatus mStatus=FadeStatus.FadeIn;
	#endregion
	
	//是否允许玩家触发渐入渐出事件
	public bool WaitForInput=true;
	//当前透明度
	private float mAlpha=0.0F;
	//摄像机
	private Camera mCam;
	private GameObject mCamObj;
	//Logo贴图位置
	private Rect mSplashLogoPos;
	//渐入结束的时间
	private float mFadeInFinishedTime;
	//关卡是否加载完毕
	private bool LevelisLoaded=false;
	
	void Start () 
	{
		//保存相机
		mCam=Camera.main;
		mCamObj=Camera.main.gameObject;
		//计算Logo绘制的位置
		mSplashLogoPos.x=(Screen.width * 0.5F-SplashLogo.width * 0.5F);
		mSplashLogoPos.y=(Screen.height * 0.5F-SplashLogo.height * 0.5F);
		mSplashLogoPos.width=SplashLogo.width;
		mSplashLogoPos.height=SplashLogo.height;
		//如果是渐出后加载关卡则保留相机
		if (Type == SplashType.LoadLevelThenFadeOut)
		{
			DontDestroyOnLoad(this);
			DontDestroyOnLoad(Camera.main);
		}
		//检查目标关卡是否为空
		if ((Application.levelCount <= 1) || (LevelToLoad == ""))
		{
			Debug.Log("There is not have the level to load please check again");
			return;
		}
	}
	
	void Update () 
	{
		switch(mStatus)
		{
		case FadeStatus.FadeIn:
			mAlpha+=FadeSpeed * Time.deltaTime;
			break;
		case FadeStatus.FadeOut:
			mAlpha-=FadeSpeed * Time.deltaTime;
			break;
		case FadeStatus.FadeWait:
			//当设定为FadeWait时可根据时间判定或者玩家触发进入下一个状态
			if((!WaitForInput && Time.time>mFadeInFinishedTime+WaitTime) || (WaitForInput && Input.anyKey))
			{
				mStatus=FadeStatus.FadeOut;
			}
			break;
		}
		
	}
	
	void OnGUI()
	{
		if(SplashLogo!=null)
		{
			//绘制Logo
			GUI.color=new Color(GUI.color.r,GUI.color.g,GUI.color.b,Mathf.Clamp(mAlpha,0,1));
			GUI.DrawTexture(mSplashLogoPos,SplashLogo);
			
			//状态判断
			if(mAlpha>1.0F)
			{
				mStatus=FadeStatus.FadeWait;
				mFadeInFinishedTime=Time.time;
				mAlpha=1.0F;
				//如果需要在渐入结束后加载关卡
				if(Type==SplashType.LoadLevelThenFadeOut)
				{
					mCam.depth=-1000;

				}
			}
			
			if(mAlpha<0.0F)
			{
				//如果需要在关卡加载完后渐出
				if(Type==SplashType.FadeOutThenLoadLevel)
				{
					Application.LoadLevel("sence2");
				}else
				{
					Destroy(mCamObj);
					Destroy(this);
				}
			}
		}
		
		if(mStatus==FadeStatus.FadeWait)
		{
			mStatus = FadeStatus.FadeOut;
			StartCoroutine("loadSence2");
			//Debug.Log("请按任意键继续");   
		}
	}

	public IEnumerator loadSence2()
	{
		yield return new WaitForSeconds(2f);
		Application.LoadLevel("sence2");

	}
	void OnLevelWasLoaded(int index)
	{
		//如果目标关卡已加载需要手动销毁场景中的GUI和AudioListener
		if(LevelisLoaded)
		{
			Destroy(mCam.GetComponent<AudioListener>());
			Destroy(mCam.GetComponent<GUILayer>());
		}
	}
	
}