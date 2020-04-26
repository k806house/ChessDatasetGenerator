using Assets.Scripts.Core.Controllers;
using Assets.Scripts.Core.Services;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class MainEnterPoint : MonoBehaviour
    {
        [SerializeField] 
        private LightController lightController = default;
        [SerializeField] 
        private CameraController cameraController = default;
        [SerializeField]
        private BoardController boardController = default;
        [SerializeField]
        private TableController tableController = default;
        [SerializeField]
        private FloorController floorController = default;

        [SerializeField]
        private StateService stateService = default;
        [SerializeField]
        private SettingsService settingsService = default;

        private void Awake()
        {
            settingsService.Init();
            cameraController.Init(settingsService, stateService, boardController, lightController);
            boardController.Init(settingsService, stateService);
            lightController.Init(settingsService);
            tableController.Init(settingsService);
            floorController.Init(settingsService);
            stateService.Init(settingsService);
        }
    }
}
