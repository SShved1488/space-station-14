using Content.Shared._Sunrise.Pacificator;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using FancyWindow = Content.Client.UserInterface.Controls.FancyWindow;

namespace Content.Client._Sunrise.Pacificator
{
    [GenerateTypedNameReferences]
    public sealed partial class PacificatorWindow : FancyWindow
    {
        private readonly ButtonGroup _buttonGroup = new();

        private readonly PacificatorBoundUserInterface _owner;

        public PacificatorWindow(PacificatorBoundUserInterface owner)
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            _owner = owner;

            OnButton.Group = _buttonGroup;
            OffButton.Group = _buttonGroup;

            OnButton.OnPressed += _ => _owner.SetPowerSwitch(true);
            OffButton.OnPressed += _ => _owner.SetPowerSwitch(false);

            EntityView.SetEntity(owner.Owner);
        }

        public void UpdateState(GeneratorState state)
        {
            if (state.On)
                OnButton.Pressed = true;
            else
                OffButton.Pressed = true;

            PowerLabel.Text = Loc.GetString(
                "gravity-generator-window-power-label",
                ("draw", state.PowerDraw),
                ("max", state.PowerDrawMax));

            PowerLabel.SetOnlyStyleClass(MathHelper.CloseTo(state.PowerDraw, state.PowerDrawMax) ? "Good" : "Caution");

            ChargeBar.Value = state.Charge;
            ChargeText.Text = (state.Charge / 255f).ToString("P0");
            StatusLabel.Text = Loc.GetString(state.PowerStatus switch
            {
                PacificatorPowerStatus.Off => "gravity-generator-window-status-off",
                PacificatorPowerStatus.Discharging => "gravity-generator-window-status-discharging",
                PacificatorPowerStatus.Charging => "gravity-generator-window-status-charging",
                PacificatorPowerStatus.FullyCharged => "gravity-generator-window-status-fully-charged",
                _ => throw new ArgumentOutOfRangeException()
            });

            StatusLabel.SetOnlyStyleClass(state.PowerStatus switch
            {
                PacificatorPowerStatus.Off => "Danger",
                PacificatorPowerStatus.Discharging => "Caution",
                PacificatorPowerStatus.Charging => "Caution",
                PacificatorPowerStatus.FullyCharged => "Good",
                _ => throw new ArgumentOutOfRangeException()
            });

            EtaLabel.Text = state.EtaSeconds >= 0
                ? Loc.GetString("gravity-generator-window-eta-value", ("left", TimeSpan.FromSeconds(state.EtaSeconds)))
                : Loc.GetString("gravity-generator-window-eta-none");

            EtaLabel.SetOnlyStyleClass(state.EtaSeconds >= 0 ? "Caution" : "Disabled");
        }
    }
}
