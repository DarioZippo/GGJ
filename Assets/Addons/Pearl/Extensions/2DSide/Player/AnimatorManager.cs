#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOM;
using Pearl;

namespace Pearl.Side2D
{
    public class AnimatorManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private CharacterManager characterManager = null;
        [SerializeField]
        private Side2DController characterController = null;
        [SerializeField]
        private SpriteRenderer spriteRender = null;
        [SerializeField]
        private Animator animator = null;


        private Color _normalColor;

        // Start is called before the first frame update
        private void Start()
        {
            if (spriteRender)
            {
                _normalColor = spriteRender.color;
            }
        }


        public void OnStamina(float percent)
        {
            if (spriteRender)
            {
                Color newColor = _normalColor * percent;
                newColor.a = 1;
                spriteRender.color = newColor;   
            }
        }

        private void Reset()
        {
            characterManager = GetComponent<CharacterManager>();
            characterController = GetComponent<Side2DController>();
            animator = GetComponent<Animator>();
            spriteRender = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            if (characterManager == null || characterController == null)
            {
                return;
            }

            float Xspeed = characterController.Speed.x;
            float Yspeed = characterController.Speed.y;
            SetBool("Alive", true);
            SetFloat("xSpeed", Xspeed);
            SetFloat("ySpeed", Yspeed);

            SetBool("Grounded", characterController.State.IsGrounded);

            if (Xspeed > 0 || Xspeed < 0)
            {
                SetBool("Walking", true);
            }
            else
            {
                SetBool("Walking", false);
                SetBool("Idle", true);
            }
        }

        public void SetBool(string id, bool value)
        {
            if (animator && animator.runtimeAnimatorController)
            {
                animator.SetBool(id, value);
            }
        }

        public void SetFloat(string id, float value)
        {
            if (animator && animator.runtimeAnimatorController)
            {
                animator.SetFloat(id, value);
            }
        }

        private void SetState()
        {
            //var state = characterManager.CurrentControllerState;
        }
    }

}

#endif