using UnityEngine;
using Labyrinth.Collectibles;
using Labyrinth.Rendering;

namespace Labyrinth.Factories
{
    public static class DiamondFactory
    {
        public static Diamond Create(Vector3 position, DiamondCollector collector, Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Diamond";
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.35f, 0.55f, 0.35f);
            go.transform.rotation = Quaternion.Euler(45f, 45f, 0f);

            Object.Destroy(go.GetComponent<Collider>());
            SphereCollider trigger = go.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.8f;

            go.GetComponent<MeshRenderer>().material = CreateDiamondMaterial();

            Diamond diamond = go.AddComponent<Diamond>();
            diamond.Initialize(collector);
            return diamond;
        }

        private static Material CreateDiamondMaterial()
        {
            var color = new Color(0.25f, 0.85f, 1f);
            return RuntimeMaterialFactory.CreateEmissive(color, 1.5f);
        }
    }
}
