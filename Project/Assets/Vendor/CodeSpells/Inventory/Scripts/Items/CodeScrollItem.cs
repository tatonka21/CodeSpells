using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CodeScrollItem : DraggableItem {
	
	public string file_name;
	
	private int current_frame = 0;
	
	private bool animate = false;
	
	private Texture2D still_icon;
	private Texture2D[] animated_icon;
	
	private bool on_drag = false;
	private float time_started;
	
	private bool compilable = true;

	
    override protected void Drag(){
        // make "uncastable" (uncompilable) spells not draggable
        if (!compilable) {
            time_started = Time.time;
            return;
        }
		if (!on_drag) {
			time_started = Time.time;
			on_drag = true;
		}
		base.Drag();
		
		GameObject obj = objectUnderCursor();
		if(obj != null){
			if(obj.GetComponent<Enchantable>() != null) {
				Highlighter.highlight(obj, Color.green);
			} else {
				Highlighter.highlight(obj, Color.red);
			}
		} else {
			Highlighter.highlight(null, Color.black);
		}
	}
	
	override public Texture2D getTexture()	
	{
	    if (!compilable) {
	        if (still_icon == null || !still_icon.ToString().Contains("UncastableScroll")) {
	            still_icon = Resources.Load("Textures/UncastableScroll") as Texture2D;
	        }
	        return still_icon;
	    }
		if(still_icon == null || still_icon.ToString().Contains("UncastableScroll"))
			still_icon = Resources.Load("Textures/Scroll") as Texture2D;
		
		if(animated_icon == null)
		{
			animated_icon = new Texture2D[3];
			animated_icon[0] = Resources.Load("SparkleScroll1") as Texture2D;
			animated_icon[1] = Resources.Load("SparkleScroll2") as Texture2D;	
			animated_icon[2] = Resources.Load("SparkleScroll3") as Texture2D;	
		}
		              
		if(animate)
		{
			current_frame = (current_frame + 1) % 30;
	
			return animated_icon[(current_frame / 10)];
		} else {
			return still_icon;
		}
	}
	
	public override void DroppedOnInventory(Vector3 mousePosition){
		on_drag = false;
		SetHidden(false);
		if ((Time.time-time_started) < 0.5f) {
			
			if(isEmpty())
				return;
			
			IDEInput input = getIDEInput();
			IDE ide = (GameObject.Find("IDE").GetComponent("IDE") as IDE);
			ide.SetInput(input);
			ide.show(GameObject.Find("Inventory"));
		}
	}
	
	public IDEInput getIDEInput(){
		UnityEngine.Debug.Log("********************IN CodeScrollITEM getIDEInput**********************");
		UnityEngine.Debug.Log("juneconfig.java_files_path: "+JuneConfig.java_files_path);
		UnityEngine.Debug.Log("file_name: "+file_name);
		UnityEngine.Debug.Log("********************DONE WITH CodeScrollITEM getIDEInput**********************");
		return new EclipseInput("CodeSpellsJava", JuneConfig.java_files_path+"/"+file_name);
	}
	
	public override void DroppedOn(GameObject target)
	{
		if(target == null)
		{
			SetHidden(false);
			return;
		}
		
		if(isEmpty())
		{
			absorbSpell(target);
			
		} else {
			castSpell(target);	
		}
		
		

	}
	
	public void absorbSpell(GameObject target)
	{
		SetHidden(false);
		
		Enchantable enchantable = (target.GetComponent("Enchantable") as Enchantable);
		
		if(enchantable == null)
		{
			SetHidden(false);

			return;
		}
		
		string file_name = enchantable.disenchant();
		
		//string file_name = enchantable.getJune().getFileName();
		setCurrentFile(file_name);

	}
	
	public void setCurrentFile(string file_name){
		this.file_name = file_name;
		this.item_name = file_name.Split('.')[0];
		this.animate = true;	
	}
	
	public string getSpellName()
	{
	    return file_name.Split('.')[0];
	}
	
	public void castSpell(GameObject target)
	{
		if(target.GetComponent("Enchantable") == null)
		{
			TraceLogger.LogKVtime("attempt", getSpellName());
			TraceLogger.LogKV("target", target.GetInstanceID().ToString()+", "+target.name+", "+target.transform.position);
            TraceLogger.LogKV("player", ""+ObjectManager.FindById("Me").transform.position);
			(GameObject.Find("Popup").GetComponent("Popup") as Popup).popup("Target ("+target.name+") immune to magic.");
			SetHidden(false);
			return;
		}
		
		TraceLogger.LogKVtime("spell", getSpellName());
		ProgramLogger.LogKVtime("spell", getSpellName());
		TraceLogger.LogKV("target", target.GetInstanceID().ToString()+", "+target.name+", "+target.transform.position);
		TraceLogger.LogKV("player", ""+ObjectManager.FindById("Me").transform.position);
		
		June june = new June(target, file_name);

		SetHidden(false);
		
		item_name = "Blank";
		file_name = "";
		animate = false;

		
		inventoryTexture = Resources.Load( "Textures/Scroll") as Texture2D;

		
		(target.GetComponent("Enchantable") as Enchantable).enchant(june, delegate(GameObject t){absorbSpell(t); });
		

	}
		   
	private bool isEmpty()
	{
		return file_name == null || file_name.Equals("");		
	}
	
	public void SetCompilable()
	{
	    Process compile_process = Shell.shell_no_start("javac", "-classpath \"" + JuneConfig.june_files_path + "\" "+JuneConfig.java_files_path+"/"+file_name);
        compile_process.Start();	
        
        var output = compile_process.StandardOutput.ReadToEnd();
        var error = compile_process.StandardError.ReadToEnd();
        
//		Popup.mainPopup.popup(""  + output + " " + error);

        if(!error.Equals(""))
            compilable = false;
        else
            compilable = true;
	}
	
	public bool IsCompilable()
	{
	    return compilable;
	}
}
		 
