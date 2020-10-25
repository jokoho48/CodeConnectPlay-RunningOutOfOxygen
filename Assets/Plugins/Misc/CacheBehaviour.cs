namespace Plugins
{
#region

    using System;
    using UnityEngine;
    using UnityEngine.AI;

#endregion

    /// <summary>
    ///     A safe, drop-in replacement for MonoBehaviour as your base class. Each property value is cached
    ///     and GetComponent will be called if the instance is null or is destroyed.
    /// </summary>
    public abstract class CacheBehaviour : MonoBehaviour
    {
        [NonSerialized] private Animation _animation;

        [NonSerialized] private AudioSource _audio;

        [NonSerialized] private Camera _camera;

        [NonSerialized] private Collider _collider;

        [NonSerialized] private Collider2D _collider2D;

        [NonSerialized] private ConstantForce _constantForce;

        [NonSerialized] private HingeJoint _hingeJoint;

        [NonSerialized] private Light _light;

        [NonSerialized] private NavMeshAgent _navMeshAgent;

        [NonSerialized] private ParticleSystem _particleSystem;

        [NonSerialized] private Renderer _renderer;

        [NonSerialized] private Rigidbody _rigidbody;

        [NonSerialized] private Rigidbody2D _rigidbody2D;

        /// <summary>
        ///     Gets the Animation attached to the object.
        /// </summary>
        public new Animation animation => _animation ? _animation : _animation = GetComponent<Animation>();

        /// <summary>
        ///     Gets the AudioSource attached to the object.
        /// </summary>
        public new AudioSource audio => _audio ? _audio : _audio = GetComponent<AudioSource>();

        /// <summary>
        ///     Gets the Camera attached to the object.
        /// </summary>
        public new Camera camera => _camera ? _camera : _camera = GetComponent<Camera>();

        /// <summary>
        ///     Gets the Collider attached to the object.
        /// </summary>
        public new Collider collider => _collider ? _collider : _collider = GetComponent<Collider>();

        /// <summary>
        ///     Gets the Collider2D attached to the object.
        /// </summary>
        public new Collider2D collider2D => _collider2D ? _collider2D : _collider2D = GetComponent<Collider2D>();

        /// <summary>
        ///     Gets the ConstantForce attached to the object.
        /// </summary>
        public new ConstantForce constantForce =>
            _constantForce ? _constantForce : _constantForce = GetComponent<ConstantForce>();

        /// <summary>
        ///     Gets the HingeJoint attached to the object.
        /// </summary>
        public new HingeJoint hingeJoint => _hingeJoint ? _hingeJoint : _hingeJoint = GetComponent<HingeJoint>();

        /// <summary>
        ///     Gets the Light attached to the object.
        /// </summary>
        public new Light light => _light ? _light : _light = GetComponent<Light>();

        /// <summary>
        ///     Gets the ParticleSystem attached to the object.
        /// </summary>
        public new ParticleSystem particleSystem =>
            _particleSystem ? _particleSystem : _particleSystem = GetComponent<ParticleSystem>();

        /// <summary>
        ///     Gets the Renderer attached to the object.
        /// </summary>
        public new Renderer renderer => _renderer ? _renderer : _renderer = GetComponent<Renderer>();

        /// <summary>
        ///     Gets the Rigidbody attached to the object.
        /// </summary>
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        /// <summary>
        ///     Gets the Rigidbody2D attached to the object.
        /// </summary>
        public new Rigidbody2D rigidbody2D => _rigidbody2D ? _rigidbody2D : _rigidbody2D = GetComponent<Rigidbody2D>();

        /// <summary>
        ///     Gets the Rigidbody2D attached to the object.
        /// </summary>
        public NavMeshAgent navMeshAgent =>
            _navMeshAgent ? _navMeshAgent : _navMeshAgent = GetComponent<NavMeshAgent>();
    }
}