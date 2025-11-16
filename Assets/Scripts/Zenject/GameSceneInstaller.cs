using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject statesContainerPrefab;
    [SerializeField] private GameObject playerInputPrefab;
    public override void InstallBindings()
    {
        Container.Bind<StatesContainer>().FromComponentInNewPrefab(statesContainerPrefab).AsSingle().NonLazy();
        Container.Bind<PlayerInput>().FromComponentInNewPrefab(playerInputPrefab).AsSingle().NonLazy();
    }
}