using LSDR.SDK.Entities;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class InteractiveObjectProxy : AbstractLuaProxy<InteractiveObject>
    {
        private readonly GameObject _gameObject;

        [MoonSharpHidden]
        public InteractiveObjectProxy(InteractiveObject target) : base(target) { _gameObject = target.gameObject; }

#region Properties

        public Vector3 Forward => _gameObject.transform.forward;
        public Vector3 Up => _gameObject.transform.up;
        public Vector3 Right => _gameObject.transform.right;

        public GameObject GameObject => _gameObject;

#endregion

#region Functions

        // public void PlayAnimation(int index)
        // {
        //     if (index >= _target.Data.Animations.Count)
        //     {
        //         Debug.LogWarning($"Unable to play animation {index} on object '{_gameObject.name}', " +
        //                          $"object only has {_target.Data.Animations.Count} animations");
        //         return;
        //     }
        //
        //     _target.Animator.Play(_target.Data.Animations[index]);
        // }
        //
        // public TODAnimation GetAnimation(int index)
        // {
        //     if (index >= _target.Data.Animations.Count)
        //     {
        //         Debug.LogWarning($"Unable to get animation {index} on object '{_gameObject.name}', " +
        //                          $"object only has {_target.Data.Animations.Count} animations");
        //         return null;
        //     }
        //
        //     return _target.Data.Animations[index];
        // }
        //
        // public void PauseAnimation() { _target.Animator.Pause(); }
        //
        // public void ResumeAnimation() { _target.Animator.Resume(); }
        //
        // public void StopAnimation() { _target.Animator.Stop(); }
        //
        // public void PlayAudio(ToriiAudioClip clip) { _target.AudioSource.PlayOneShot(clip); }
        //
        // public void SetPosition(Vector3 position) { _gameObject.transform.position = position; }
        //
        // public void Translate(Vector3 vec) { _gameObject.transform.Translate(vec, Space.Self); }
        //
        // public void SetRotation(Vector3 euler) { _gameObject.transform.eulerAngles = euler; }
        //
        // public void Rotate(Vector3 euler) { _gameObject.transform.Rotate(euler); }
        //
        // public void RotateAngleAxis(float angle, Vector3 axis) { _gameObject.transform.Rotate(axis, angle); }
        //
        // public void SetScale(Vector3 scale) { _gameObject.transform.localScale = scale; }

#endregion
    }
}
