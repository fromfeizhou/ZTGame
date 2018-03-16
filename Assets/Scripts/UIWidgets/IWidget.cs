
public interface IWidget
{
	#if UNITY_EDITOR 
	void InitEditor(string paramStr);
	#endif
	
	//初始化控件
	void Init (string paramStr);

	WidgetAnimation WidgetAnim{ get;}


}
