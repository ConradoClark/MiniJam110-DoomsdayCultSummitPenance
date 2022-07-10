using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.MainMenu
{
    public class StartGame : MenuButtonFunction
    {
        private MainMenuEffects _mainMenuEffects;
        protected override void OnAwake()
        {
            base.OnAwake();
            _mainMenuEffects = SceneObject<MainMenuEffects>.Instance();

        }

        public string SceneToLoad;
        public override IEnumerable<IEnumerable<Action>> Execute()
        {
            yield return _mainMenuEffects.HideButtons().AsCoroutine();

            DefaultMachinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
            });
        }
    }
}
