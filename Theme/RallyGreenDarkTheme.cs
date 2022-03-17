using MudBlazor;
namespace BoostMyEconomy.Theme
{
    public class RallyGreenDarkTheme : MudTheme
    {
        //public Breakpoints Breakpoints { get; set; }
        public new Palette Palette { get; set; }
        public new Palette PaletteDark { get; set; }
        public new Shadow Shadows { get; set; }
        public new Typography Typography { get; set; }
        public new LayoutProperties LayoutProperties { get; set; }
        public new ZIndex ZIndex { get; set; }


        public RallyGreenDarkTheme()
        {
            Palette = new Palette()
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
            PaletteDark = Palette;
            Shadows = new Shadow();
            Typography = new Typography();
            LayoutProperties = new LayoutProperties();
            ZIndex = new ZIndex();
        }
    }
}
