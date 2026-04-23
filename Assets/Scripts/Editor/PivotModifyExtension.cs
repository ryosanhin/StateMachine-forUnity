using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PivotModifyExtension : Editor
{	
	[MenuItem("CONTEXT/Image/Modify RectTransform Pivot to Sprite Pivot")]
	static void ModifyPivot(MenuCommand command){
		var image=command.context as Image;
		var sprite = image.sprite;
		
		if(sprite is null){
			Debug.LogWarning("Sprite is null");
			return;
		}
		
		var rectTransform = image.transform as RectTransform;
		
		var newPivot=new Vector2(
			sprite.pivot.x/sprite.rect.width,
			sprite.pivot.y/sprite.rect.height
		);
		
		Undo.RecordObject(rectTransform, "Modify RectTransform Pivot");
		
		rectTransform.pivot=newPivot;

        EditorUtility.SetDirty(rectTransform);
	}
}
