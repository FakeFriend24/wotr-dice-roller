﻿/* 
 * Directly and shamelessly stolen from and based on Bubble's BubbleBuffs: https://github.com/factubsio/BubbleBuffs/blob/master/BubbleBuffs/Utilities/Searchbar.cs  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Owlcat.Runtime.UI.Controls.Button;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DiceRollerWotR
{

    public class SearchBar
    {
        public GameObject RootGameObject;
        public TMP_InputField InputField;
        public OwlcatButton InputButton;
        public TMP_Dropdown Dropdown;
        public OwlcatButton DropdownButton;
        public GameObject DropdownIconObject;
        public TextMeshProUGUI PlaceholderText;

        public SearchBar(Transform parent, string placeholder, string startValue = "", string name = "DiceRoller_SearchBar", IEnumerable<string> dropDownOptions = null, int startIndex = 1, UnityAction<int> onSelection = null, UnityAction<string> onValueChanged = null)
        {

            if (Accessor.FeatureSearchPrefab == null)
            {
                string err = "Error: Unable to locate search bar prefab, it's likely a patch has changed the UI setup, or you are in an unexpected situation. Please report this bug!";
                Log.Error(err);
                throw new UnityException(err);
            }

            RootGameObject = GameObject.Instantiate(Accessor.FeatureSearchPrefab, parent, false).gameObject;
            RootGameObject.name = name;

            InputButton = RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/Placeholder").GetComponent<OwlcatButton>();
            if (dropDownOptions != null)
            {
                Dropdown = RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/Dropdown").GetComponent<TMP_Dropdown>();
                DropdownButton = RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/Dropdown/GenerateButtonPlace").GetComponent<OwlcatButton>();
                DropdownIconObject = RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/Dropdown/GenerateButtonPlace/GenerateButton/Icon").gameObject;
            }
            else
            {
                GameObject.Destroy(RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/Dropdown").gameObject);
            }
            PlaceholderText = RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/Placeholder/Label").GetComponent<TextMeshProUGUI>();
            InputField = RootGameObject.transform.Find("FieldPlace/SearchField/SearchBackImage/InputField").GetComponent<TMP_InputField>();

            InputField.onValueChanged.AddListener(delegate (string _) { OnInputFieldEdit(); });
            InputField.onValueChanged.AddListener(onValueChanged);
            InputField.onEndEdit.AddListener(delegate (string _) { OnInputFieldEditEnd(); });
            InputButton.OnLeftClick.AddListener(delegate { OnInputClick(); });
            if (dropDownOptions != null)
            {
                Dropdown.onValueChanged.AddListener(delegate (int _) { OnDropdownSelected(); });
                DropdownButton.OnLeftClick.AddListener(delegate { OnDropdownButton(); });
            }

            GameObject.Destroy(RootGameObject.GetComponent<CharGenFeatureSearchPCView>()); // controller from where we stole the search bar
            InputField.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().SetText(placeholder);
            PlaceholderText.text = Accessor.Settings.DiceExpression;
            InputField.interactable = true;
            if (dropDownOptions != null)
            {
                Dropdown.ClearOptions();
                Dropdown.AddOptions(new List<string>(dropDownOptions));
                if(onSelection != null)
                {
                    Dropdown.onValueChanged.AddListener(onSelection);
                }
                this.Dropdown.value = startIndex;
                GameObject.Destroy(Dropdown.template.Find("Viewport/TopBorderImage").gameObject);
                Transform border = Dropdown.template.Find("Viewport/Content/Item/BottomBorderImage");
                RectTransform rect = border.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.0f, 0.0f);
                rect.anchorMax = new Vector2(1.0f, 0.0f);
                rect.offsetMin = new Vector2(0.0f, 0.0f);
                rect.offsetMax = new Vector2(0.0f, 2.0f);
            }
            RootGameObject.AddComponent<LayoutElement>().flexibleWidth = 3;

            try { 
            InputField.text = startValue;
            } catch(Exception e)
            {
                Log.Error(e);
            }
        }

        public void FocusSearchBar()
        {
            OnInputClick();
        }

        public void UpdatePlaceholder()
        {
            PlaceholderText.text = string.IsNullOrEmpty(InputField.text) ? InputField.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text : InputField.text;
            

        }

        private void OnDropdownButton()
        {
            Dropdown.Show();
        }

        private void OnDropdownSelected()
        {
            UpdatePlaceholder();
        }

        private void OnInputClick()
        {
            InputButton.gameObject.SetActive(false);
            InputField.gameObject.SetActive(true);
            InputField.Select();
            InputField.ActivateInputField();
        }

        private void OnInputFieldEdit()
        {
            UpdatePlaceholder();
        }

        private void OnInputFieldEditEnd()
        {
            InputField.gameObject.SetActive(false);
            InputButton.gameObject.SetActive(true);

            if (!EventSystem.current.alreadySelecting) // could be, in same click, ending edit and starting dropdown
            {
                EventSystem.current.SetSelectedGameObject(RootGameObject); // return focus to regular UI
            }
        }
    }
}
