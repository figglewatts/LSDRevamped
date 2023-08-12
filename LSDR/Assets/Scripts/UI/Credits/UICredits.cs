using System.Collections.Generic;
using LSDR.InputManagement;
using Torii.Event;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.UI.Credits
{
    public class UICredits : MonoBehaviour
    {
        public const string CREDITS_FILE_PATH = "credits.json";
        public GameObject UICreditsSectionPrefab;
        public GameObject UITitleTextPrefab;
        public GameObject UIEndCapObject;
        public RectTransform UISectionContainer;
        public float ScrollSpeed = 0.2f;
        public ToriiEvent OnCreditsEnd;
        public ControlSchemeLoaderSystem Controls;

        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private bool _creditsFinished;

        public void Start() { load(); }

        public void Update()
        {
            if (Controls.InputActions.Game.Run.IsPressed())
            {
                OnCreditsEnd.Raise();
                _creditsFinished = true;
            }

            if (!_creditsFinished && UISectionContainer.anchoredPosition.y > UISectionContainer.rect.height + 500)
            {
                OnCreditsEnd.Raise();
                _creditsFinished = true;
            }
        }

        public void FixedUpdate() { UISectionContainer.anchoredPosition += new Vector2(x: 0, ScrollSpeed); }

        public void OnEnable()
        {
            UISectionContainer.anchoredPosition = new Vector2(UISectionContainer.anchoredPosition.x, y: 0);
            _creditsFinished = false;
        }

        private void load()
        {
            var creditsSections =
                _serializer.Deserialize<List<CreditsSection>>(PathUtil.Combine(Application.streamingAssetsPath,
                    CREDITS_FILE_PATH));

            addObject(UITitleTextPrefab);

            foreach (CreditsSection section in creditsSections) createSection(section);

            addObject(UIEndCapObject);
        }

        private void createSection(CreditsSection section)
        {
            UICreditsSection sectionScript = addObject(UICreditsSectionPrefab).GetComponent<UICreditsSection>();
            sectionScript.SetTitle(section.SectionTitle);
            sectionScript.SetImage(section.SectionImagePath);
            sectionScript.PopulateNames(section.Names, section.SortNames);
        }

        private GameObject addObject(GameObject obj) { return Instantiate(obj, UISectionContainer); }
    }
}
