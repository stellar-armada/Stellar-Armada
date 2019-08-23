 using System.Collections.Generic;
using System.Text;
 using StellarArmada.UI;
 using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649
 namespace StellarArmada.Ships
 {
     public class SelectionTest : MonoBehaviour
     {
         [SerializeField] Text currentSelectionText;
         [SerializeField] private Text[] selectionSetTexts;

         public void UpdateCurrentlySelected()
         {
             StringBuilder stringBuilder = new StringBuilder();

             foreach (ISelectable selectable in SelectionUIManager.instance.GetCurrentSelection())
             {
                 stringBuilder.AppendLine(selectable.GetOwningEntity().GetGameObject().name);
             }

             currentSelectionText.text = stringBuilder.ToString();
         }

         public void UpdateSelectionSets()
         {
             StringBuilder stringBuilder;
             for (int i = 0; i < selectionSetTexts.Length; i++)
             {
                 stringBuilder = new StringBuilder();

                 List<ISelectable> selection = SelectionUIManager.instance.GetSelectionSet(i);
                 if (selection == null) continue;
                 foreach (ISelectable selectable in selection)
                 {
                     stringBuilder.AppendLine(selectable.GetOwningEntity().GetGameObject().name);
                 }

                 selectionSetTexts[i].text = stringBuilder.ToString();
             }
         }

     }
 }