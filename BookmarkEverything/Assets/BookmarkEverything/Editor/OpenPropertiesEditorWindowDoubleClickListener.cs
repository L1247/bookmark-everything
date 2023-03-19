 
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
 
namespace JD.AssetizerEditor
{
   /// <summary>
   /// Listens <see cref="OnOpenAssetAttribute"/> (order 100) for everything except folders.
   /// Opens the asset in a new window (like an unchangeable inspector).
   /// Forum thread: https://forum.unity.com/threads/alt-p-is-a-godsent.834703/
   /// </summary>
   public static class OpenPropertiesEditorWindowDoubleClickListener
   {
      private static MethodInfo openPropertyEditorInfo;
      private static System.Type[] callTypes = new[] { typeof(Object), typeof(bool) };
      private static object[] callOpenBuffer = { null, true };
 
      /// <summary>
      /// Listens <see cref="OnOpenAssetAttribute"/> (order 100) for everything except folders.
      /// </summary>
      /// <param name="instanceID"><see cref="OnOpenAssetAttribute"/></param>
      /// <param name="line"><see cref="OnOpenAssetAttribute"/></param>
      /// <returns>True if opening the asset is handled</returns>
      [OnOpenAsset(100)]
      private static bool HandleOpenAsset(int instanceID, int line)
      {
         Object obj = EditorUtility.InstanceIDToObject(instanceID);
         if (obj == null)
         {
            return false;
         }
 
         if (IsFolder(obj))
         {
            return false;
         }
 
         return OpenInPropertyEditor(obj);
      }
 
      private static bool IsFolder(Object obj)
      {
         string assetPath = AssetDatabase.GetAssetPath(obj);
         return !string.IsNullOrEmpty(assetPath) && AssetDatabase.IsValidFolder(assetPath);
      }

      public static bool OpenInPropertyEditor(Object asset)
      {
         if (openPropertyEditorInfo == null)
         {
            System.Type propertyEditorType = typeof(Editor).Assembly.GetType("UnityEditor.PropertyEditor");
 
            // Get specific method, since there is an overload starting with Unity 2021.2
            openPropertyEditorInfo = propertyEditorType.GetMethod(
               "OpenPropertyEditor",
               BindingFlags.Static | BindingFlags.NonPublic,
               null,
               callTypes,
               null);
         }
 
 
         if (openPropertyEditorInfo != null)
         {
            callOpenBuffer[0] = asset;
            openPropertyEditorInfo.Invoke(null, callOpenBuffer);
            return true;
         }
 
         return false;
      }
   }
}