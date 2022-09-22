//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/Input/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player Movement"",
            ""id"": ""fedeb6be-7602-4b9a-a7ef-02ea480be561"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3378ed76-f7ff-448e-b53c-71988292cb91"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""6d022864-1742-482e-9daf-bef20f8487fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""03cf4991-26d3-4771-b62d-be700365264f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Switch Unit"",
                    ""type"": ""Button"",
                    ""id"": ""e3b23e42-e2af-42a6-b69f-a4c01c42eb44"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotate Camera"",
                    ""type"": ""Value"",
                    ""id"": ""dac07e50-ed37-43b8-860a-9daf310ad6bf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Open Weapon Selector"",
                    ""type"": ""Button"",
                    ""id"": ""dd7fdae4-73a4-4a8d-a67e-0bf928b99667"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""f8d4a47d-fc11-4491-96e4-5868e5f4a847"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e07a8969-24d2-4408-956d-018d905a0a60"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""951e643f-afca-4832-87f6-85e79cb7f7fc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fec26be5-0779-4cbf-8fd3-865f2bcf1c10"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c3d09147-33fa-4b25-94d7-478de5203952"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2ac63f4e-e509-402e-b772-cf900b7406da"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b01cac04-b4cf-42fb-accb-5c6c664223c3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6ad02a9c-8818-4fb9-a901-c5e5b0030eed"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Switch Unit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""004ce573-2d59-46c6-b7b0-58f89e8c7cbc"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86f4f3cf-bd09-4398-95ff-56dcddeb2dbb"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Open Weapon Selector"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player Movement
        m_PlayerMovement = asset.FindActionMap("Player Movement", throwIfNotFound: true);
        m_PlayerMovement_Move = m_PlayerMovement.FindAction("Move", throwIfNotFound: true);
        m_PlayerMovement_Jump = m_PlayerMovement.FindAction("Jump", throwIfNotFound: true);
        m_PlayerMovement_Shoot = m_PlayerMovement.FindAction("Shoot", throwIfNotFound: true);
        m_PlayerMovement_SwitchUnit = m_PlayerMovement.FindAction("Switch Unit", throwIfNotFound: true);
        m_PlayerMovement_RotateCamera = m_PlayerMovement.FindAction("Rotate Camera", throwIfNotFound: true);
        m_PlayerMovement_OpenWeaponSelector = m_PlayerMovement.FindAction("Open Weapon Selector", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player Movement
    private readonly InputActionMap m_PlayerMovement;
    private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
    private readonly InputAction m_PlayerMovement_Move;
    private readonly InputAction m_PlayerMovement_Jump;
    private readonly InputAction m_PlayerMovement_Shoot;
    private readonly InputAction m_PlayerMovement_SwitchUnit;
    private readonly InputAction m_PlayerMovement_RotateCamera;
    private readonly InputAction m_PlayerMovement_OpenWeaponSelector;
    public struct PlayerMovementActions
    {
        private @Controls m_Wrapper;
        public PlayerMovementActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerMovement_Move;
        public InputAction @Jump => m_Wrapper.m_PlayerMovement_Jump;
        public InputAction @Shoot => m_Wrapper.m_PlayerMovement_Shoot;
        public InputAction @SwitchUnit => m_Wrapper.m_PlayerMovement_SwitchUnit;
        public InputAction @RotateCamera => m_Wrapper.m_PlayerMovement_RotateCamera;
        public InputAction @OpenWeaponSelector => m_Wrapper.m_PlayerMovement_OpenWeaponSelector;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Shoot.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnShoot;
                @SwitchUnit.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSwitchUnit;
                @SwitchUnit.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSwitchUnit;
                @SwitchUnit.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSwitchUnit;
                @RotateCamera.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRotateCamera;
                @RotateCamera.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRotateCamera;
                @RotateCamera.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRotateCamera;
                @OpenWeaponSelector.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnOpenWeaponSelector;
                @OpenWeaponSelector.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnOpenWeaponSelector;
                @OpenWeaponSelector.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnOpenWeaponSelector;
            }
            m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @SwitchUnit.started += instance.OnSwitchUnit;
                @SwitchUnit.performed += instance.OnSwitchUnit;
                @SwitchUnit.canceled += instance.OnSwitchUnit;
                @RotateCamera.started += instance.OnRotateCamera;
                @RotateCamera.performed += instance.OnRotateCamera;
                @RotateCamera.canceled += instance.OnRotateCamera;
                @OpenWeaponSelector.started += instance.OnOpenWeaponSelector;
                @OpenWeaponSelector.performed += instance.OnOpenWeaponSelector;
                @OpenWeaponSelector.canceled += instance.OnOpenWeaponSelector;
            }
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);
    public interface IPlayerMovementActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnSwitchUnit(InputAction.CallbackContext context);
        void OnRotateCamera(InputAction.CallbackContext context);
        void OnOpenWeaponSelector(InputAction.CallbackContext context);
    }
}
