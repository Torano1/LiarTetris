using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public class Block : MonoBehaviour
    {
        Material material;

        // Start is called before the first frame update
        void OnEnable()
        {
            InitializeBlock();
        }

        private void InitializeBlock()
        {
            if (this.material == null)
            {
                this.material = gameObject.GetComponent<Renderer>()?.material;
            }

            if (material)
            {
                material.SetFloat("_DissolveAmount", 0f);
            }
        }

        public void ChangeColor(Color color)
        {
            if (material)
            {
                // todo: use MaterialPropertyBlock instead
                material.SetColor("_BaseColor", color);
            }
        }

        public void Move(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }

        public void SetLiarTetrisMode(bool enabled)
        {
            if (material)
            {
                material.SetFloat("_DisplacementAmount", enabled ? 1 : 0);
            }
        }

        public void Dissolve(float amount)
        {
            if (material)
            {
                material.SetFloat("_DissolveAmount", Mathf.Clamp(amount, 0f, 1f));
            }
        }
    }
}