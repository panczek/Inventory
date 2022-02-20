using Code.UI;
using UnityEngine;
using Zenject;

namespace Code
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ItemController itemCtrl;
        [SerializeField] private InventoryController inventoryCtrl;
        [SerializeField] private DescriptionController descriptionCtrl;

        public override void InstallBindings()
        {
            //Container.BindInterfacesAndSelfTo<LevelObjectsController>().AsSingle();

            Container.BindInstance( itemCtrl ).AsSingle();
            Container.BindInstance( inventoryCtrl ).AsSingle();
            Container.BindInstance( descriptionCtrl ).AsSingle();
        }
    }
}
