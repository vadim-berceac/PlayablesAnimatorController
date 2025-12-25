using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject statesContainerPrefab;
    [SerializeField] private GameObject playerInputPrefab;
    [SerializeField] private GameObject characterSelectorPrefab;
    [SerializeField] private GameObject mainCameraPrefab;
    public override void InstallBindings()
    {
        Container.Bind<StatesContainer>().FromComponentInNewPrefab(statesContainerPrefab).AsSingle().NonLazy();
        Container.Bind<AvatarMasksContainer>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerInput>().FromComponentInNewPrefab(playerInputPrefab).AsSingle().NonLazy();
        Container.Bind<CharacterSelector>().FromComponentInNewPrefab(characterSelectorPrefab).AsSingle().NonLazy();
        Container.Bind<Camera>().FromComponentInNewPrefab(mainCameraPrefab).AsSingle().NonLazy();
    }
}