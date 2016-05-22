//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2013-2014 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;
using Edelweiss.Pattern;

	/// <summary>
	/// Safe coroutine to overcome deficiencies in Unity's coroutines.
	/// </summary>
	public class SafeCoroutine {

		private readonly ModifiablePropertyChangeNotifier <SafeCoroutine, SafeCoroutineState> m_StateChangeNotifier;
		/// <summary>
		/// Gets the state change notifier.
		/// </summary>
		/// <value>The state change notifier.</value>
		public PropertyChangeNotifier <SafeCoroutine, SafeCoroutineState> StateChangeNotifier {
			get {
				return (m_StateChangeNotifier.PropertyChangeNotifier);
			}
		}

		/// <summary>
		/// Gets the current state of this coroutine.
		/// </summary>
		/// <value>The current state.</value>
		public SafeCoroutineState State {
			get {
				return (m_StateChangeNotifier.Value);
			}
			protected set {
				m_StateChangeNotifier.Value = value;

				if (State != SafeCoroutineState.Paused) {
					m_IsSelfPaused = false;
					m_IsParentPaused = false;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
		public bool IsRunning {
			get {
				return (State == SafeCoroutineState.Running);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is paused.
		/// </summary>
		/// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
		public bool IsPaused {
			get {
				return (IsSelfPaused || IsParentPaused);
			}
		}

		private bool m_IsSelfPaused = false;
		/// <summary>
		/// Gets a value indicating whether this instance is self paused.
		/// </summary>
		/// <value><c>true</c> if this instance is self paused; otherwise, <c>false</c>.</value>
		public bool IsSelfPaused {
			get {
				return (m_IsSelfPaused);
			}
		}

		private bool m_IsParentPaused = false;
		/// <summary>
		/// Gets a value indicating whether this instance is parent paused.
		/// </summary>
		/// <value><c>true</c> if this instance is parent paused; otherwise, <c>false</c>.</value>
		public bool IsParentPaused {
			get {
				return (m_IsParentPaused);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has finished.
		/// </summary>
		/// <value><c>true</c> if this instance has finished; otherwise, <c>false</c>.</value>
		public bool HasFinished {
			get {
				return (State == SafeCoroutineState.Finished);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is stopped.
		/// </summary>
		/// <value><c>true</c> if this instance is stopped; otherwise, <c>false</c>.</value>
		public bool IsStopped {
			get {
				return (State == SafeCoroutineState.Stopped);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Edelweiss.Coroutine.SafeCoroutine"/> threw an exception.
		/// </summary>
		/// <value><c>true</c> if threw an exception; otherwise, <c>false</c>.</value>
		public bool ThrewException {
			get {
				return (State == SafeCoroutineState.ThrewException);
			}
		}
		
		protected internal IEnumerator m_WrappedCoroutine;
		internal virtual IEnumerator WrappedCoroutine {
			get {
				if (State != SafeCoroutineState.Created) {
					throw (new InvalidOperationException ("The wrapped coroutine can only be queried after the creation."));
				}

				IEnumerator l_Result = WrappingCoroutine (m_WrappedCoroutine);
				return (l_Result);
			}
		}
		
		private bool m_CatchExceptions = false;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Edelweiss.Coroutine.SafeCoroutine"/> catches exceptions.
		/// </summary>
		/// <value><c>true</c> if exceptions are cought; otherwise, <c>false</c>.</value>
		public bool CatchExceptions {
			get {
				return (m_CatchExceptions);
			}
			set {
				m_CatchExceptions = value;
			}
		}
		
		protected Exception m_Exception;
		/// <summary>
		/// Gets the thrown exception.
		/// </summary>
		/// <value>The thrown exception.</value>
		public Exception ThrownException {
			get {
				if (State != SafeCoroutineState.ThrewException) {
					throw (new InvalidOperationException ("Exception is only available if the coroutine is in the aborted state."));
				}
				return (m_Exception);
			}
		}

		internal SafeCoroutine (IEnumerator a_WrappedCoroutine) {
			m_StateChangeNotifier = new ModifiablePropertyChangeNotifier <SafeCoroutine, SafeCoroutineState> (this);
			State = SafeCoroutineState.Created;
			m_WrappedCoroutine = a_WrappedCoroutine;
		}

		/// <summary>
		/// Pause this instance.
		/// </summary>
		public void Pause () {
			if (State != SafeCoroutineState.Running) {
				if (!(State == SafeCoroutineState.Paused && !IsSelfPaused)) {
					throw (new InvalidOperationException ("Only running coroutines can be paused."));
				}
			}
			m_IsSelfPaused = true;
			State = SafeCoroutineState.Paused;

			PauseChildCoroutines ();
		}

		private void PauseChildCoroutines () {
			IEnumerator l_Enumerator = m_WrappedCoroutine;
			while (l_Enumerator != null) {
				object l_Yield = l_Enumerator.Current;
				if (l_Yield != null) {
					if (l_Yield is IEnumerator) {
						l_Enumerator = l_Yield as IEnumerator;
					} else if (l_Yield is SafeCoroutine) {
						SafeCoroutine l_SafeCoroutine = l_Yield as SafeCoroutine;
						if (l_SafeCoroutine.State == SafeCoroutineState.Running) {
							l_SafeCoroutine.m_IsParentPaused = true;
							l_SafeCoroutine.State = SafeCoroutineState.Paused;
							l_Enumerator = l_SafeCoroutine.m_WrappedCoroutine;

						} else if (l_SafeCoroutine.State == SafeCoroutineState.Paused) {

								// Paused means in that context self paused.
							l_SafeCoroutine.m_IsParentPaused = true;
							l_Enumerator = null;
						} else {
							l_Enumerator = null;
						}
					} else {
						
							// Stop the loop.
						l_Enumerator = null;
					}
				} else {
					
						// Stop the loop.
					l_Enumerator = null;
				}
			}
		}

		/// <summary>
		/// Resume this paused instance.
		/// </summary>
		public void Resume () {
			if
				(State != SafeCoroutineState.Paused &&
				 IsSelfPaused)
			{
				throw (new InvalidOperationException ("Only self paused coroutines can be resumed."));
			}
			m_IsSelfPaused = false;

			if (!IsParentPaused) {
				State = SafeCoroutineState.Running;
				ResumeChildCoroutines ();
			}
		}

		private void ResumeChildCoroutines () {
			IEnumerator l_Enumerator = m_WrappedCoroutine;
			while (l_Enumerator != null) {
				object l_Yield = l_Enumerator.Current;
				if (l_Yield != null) {
					if (l_Yield is IEnumerator) {
						l_Enumerator = l_Yield as IEnumerator;
					} else if (l_Yield is SafeCoroutine) {
						SafeCoroutine l_SafeCoroutine = l_Yield as SafeCoroutine;

						if (l_SafeCoroutine.IsPaused) {
							l_SafeCoroutine.m_IsParentPaused = false;
							if (l_SafeCoroutine.IsSelfPaused) {
								l_Enumerator = null;
							} else {
								l_SafeCoroutine.State = SafeCoroutineState.Running;
								l_Enumerator = l_SafeCoroutine.m_WrappedCoroutine;
							}
						} else {
							l_Enumerator = null;
						}
					} else {
						
							// Stop the loop.
						l_Enumerator = null;
					}
				} else {
					
						// Stop the loop.
					l_Enumerator = null;
				}
			}
		}

		/// <summary>
		/// Stop this instance.
		/// </summary>
		public void Stop () {
			if
				(State != SafeCoroutineState.Running &&
				 State != SafeCoroutineState.Paused)
			{
				throw (new InvalidOperationException ("Only running or paused coroutines can be stopped."));
			}
			State = SafeCoroutineState.Stopped;
			StopChildRoutines ();
		}

		private void StopChildRoutines () {
			IEnumerator l_Enumerator = m_WrappedCoroutine;
			while (l_Enumerator != null) {
				object l_Yield = l_Enumerator.Current;
				if (l_Yield != null) {
					if (l_Yield is IEnumerator) {
						l_Enumerator = l_Yield as IEnumerator;
					} else if (l_Yield is SafeCoroutine) {
						SafeCoroutine l_SafeCoroutine = l_Yield as SafeCoroutine;
						if
							(l_SafeCoroutine.State == SafeCoroutineState.Running ||
							 l_SafeCoroutine.State == SafeCoroutineState.Paused)
						{
							l_SafeCoroutine.Stop ();
						}

							// The safe coroutine takes care of further child coroutines.
						l_Enumerator = null;
					} else {

							// Stop the loop.
						l_Enumerator = null;
					}
				} else {

						// Stop the loop.
					l_Enumerator = null;
				}
			}
		}
		
		private IEnumerator WrappingCoroutine (IEnumerator a_Coroutine) {
			State = SafeCoroutineState.Running;
			while (true) {
				if (State == SafeCoroutineState.Paused) {
					yield return (null);
				} else {
					if (State == SafeCoroutineState.Stopped) {
						m_WrappedCoroutine = null;
						yield break;
					}

					object l_Yield = a_Coroutine.Current;
					SafeCoroutine l_EncapsulatedSafeCoroutine = l_Yield as SafeCoroutine;
					bool l_HasEncapsulatedSafeCoroutine =
						(l_EncapsulatedSafeCoroutine != null &&
						 l_EncapsulatedSafeCoroutine.m_WrappedCoroutine != null);

					if (!l_HasEncapsulatedSafeCoroutine) {
						try {
							if (!a_Coroutine.MoveNext ()) {
								State = SafeCoroutineState.Finished;
								m_WrappedCoroutine = null;
								yield break;
							}
						} catch (Exception l_Exception) {
							State = SafeCoroutineState.ThrewException;
							m_Exception = l_Exception;
							m_WrappedCoroutine = null;
							if (!CatchExceptions) {
								throw;
							}
							yield break;
						}
						yield return (a_Coroutine.Current);
					} else {
						yield return (null);
					}
				}
			}
		}
	}


	/// <summary>
	/// Safe coroutine to overcome deficiencies in Unity's coroutines with support for return values.
	/// </summary>
	public class SafeCoroutine <G> : SafeCoroutine {
	
		private readonly ModifiablePropertyChangeNotifier <SafeCoroutine <G>, SafeCoroutineState> m_StateChangeNotifier;
		/// <summary>
		/// Gets the generic state change notifier.
		/// </summary>
		/// <value>The generic state change notifier.</value>
		public PropertyChangeNotifier <SafeCoroutine <G>, SafeCoroutineState> GenericStateChangeNotifier {
			get {
				return (m_StateChangeNotifier.PropertyChangeNotifier);
			}
		}
	
		internal override IEnumerator WrappedCoroutine {
			get {
				if (State != SafeCoroutineState.Created) {
					throw (new InvalidOperationException ("The wrapped coroutine can only be queried after the creation."));
				}

				IEnumerator l_Result = WrappingCoroutine (m_WrappedCoroutine);
				return (l_Result);
			}
		}
	
		private G m_Result;
		/// <summary>
		/// Gets the result.
		/// </summary>
		/// <value>The result.</value>
		public G Result {
			get {
				if (!HasResult) {
					throw (new InvalidOperationException ("The result is can only be queried if HasResult is true."));
				}
				return (m_Result);
			}
		}
		
		private bool m_HasResult = false;
		/// <summary>
		/// Gets a value indicating whether this instance has result.
		/// </summary>
		/// <value><c>true</c> if this instance has result; otherwise, <c>false</c>.</value>
		public bool HasResult {
			get {
				return (HasFinished && m_HasResult);
			}
		}

		internal SafeCoroutine (IEnumerator a_InternalCoroutine) : base (a_InternalCoroutine) {
			m_StateChangeNotifier = new ModifiablePropertyChangeNotifier <SafeCoroutine <G>, SafeCoroutineState> (this);
			StateChangeNotifier.Subscribe (PropagateStateChange);
		
			if (typeof (IEnumerator).IsAssignableFrom (typeof (G)) || typeof (SafeCoroutine).IsAssignableFrom (typeof (G))) {
				throw (new InvalidOperationException ("The generic type is not allowed to be as subclass of SafeCoroutine and is also not allowed to implement the IEnumerator interface."));
			}
		}
		
		private void PropagateStateChange (SafeCoroutineState a_State) {
			m_StateChangeNotifier.Value = a_State;
		}
				
		private IEnumerator WrappingCoroutine (IEnumerator a_Coroutine) {
			State = SafeCoroutineState.Running;
			while (true) {
				if (State == SafeCoroutineState.Paused) {
					yield return (null);
				} else {
					if (State == SafeCoroutineState.Stopped) {
						m_WrappedCoroutine = null;
						yield break;
					}
					
					object l_Yield = a_Coroutine.Current;
					SafeCoroutine l_EncapsulatedSafeCoroutine = l_Yield as SafeCoroutine;
					bool l_HasEncapsulatedSafeCoroutine =
						(l_EncapsulatedSafeCoroutine != null &&
						 l_EncapsulatedSafeCoroutine.m_WrappedCoroutine != null);
					
					if (!l_HasEncapsulatedSafeCoroutine) {
						try {
							if (l_Yield is G) {
								m_HasResult = true;
								m_Result = (G) l_Yield;
							}
							if (!a_Coroutine.MoveNext ()) {
								m_WrappedCoroutine = null;
								State = SafeCoroutineState.Finished;
								yield break;
							}
						} catch (Exception l_Exception) {
							m_Exception = l_Exception;
							m_WrappedCoroutine = null;
							State = SafeCoroutineState.ThrewException;
							if (!CatchExceptions) {
								throw;
							}
							yield break;
						}
						yield return (a_Coroutine.Current);
					} else {
						yield return (null);
					}
				}
			}
		}
	}
