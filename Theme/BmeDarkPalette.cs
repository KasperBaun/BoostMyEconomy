using MudBlazor;
namespace BoostMyEconomy.Theme

{
    public class BmeDarkPalette : Palette
    {
        public Palette DarkPalette { get; }

        public BmeDarkPalette()
        {
            DarkPalette = new Palette()
            {
                AppbarBackground = Colors.BlueGrey.Darken3,
                AppbarText = Colors.Grey.Default,
                TextPrimary = Colors.Shades.White,
                TextSecondary = Colors.Shades.White,
                DrawerBackground = Colors.BlueGrey.Darken3,
                DrawerText = Colors.Green.Default,
                Background = Colors.BlueGrey.Darken1,
                Surface = Colors.BlueGrey.Darken3,
                Primary = Colors.Green.Default,
                Secondary = Colors.Green.Darken1,
                Tertiary = Colors.Green.Darken1,
                Info = Colors.Green.Darken1,
                Success = Colors.Green.Darken1,
                Warning = Colors.Green.Darken1,
                Error = Colors.Green.Darken1
            };
        }
    }
}

