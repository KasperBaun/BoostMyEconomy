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
                Background = Colors.Grey.Default,
                Surface = Colors.Grey.Darken1,
                Primary = Colors.Green.Darken4,
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
