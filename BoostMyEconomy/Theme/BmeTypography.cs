using MudBlazor;
namespace BoostMyEconomy.Theme

{
    public class BmeTypography : Typography
    {
        private Typography _bmeTypography;
        public Typography Typography => _bmeTypography;

        public BmeTypography()
        {
            _bmeTypography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = new[] { "RobotoCondensed" }
                },

                H1 = new H1()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "1.25rem",
                    FontWeight = 100,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },

                H2 = new H2()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "5.25rem",
                    FontWeight = 200,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },

                H3 = new H3()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "2.44rem",
                    FontWeight = 400,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },

                H4 = new H4()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "1.9rem",
                    FontWeight = 500,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },

                H5 = new H5()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "1.34rem",
                    FontWeight = 700,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },

                H6 = new H6()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "1.25rem",
                    FontWeight = 400,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },

                Body1 = new Body1()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "1rem",
                    FontWeight = 400,
                },

                Body2 = new Body2()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "0.9rem",
                    FontWeight = 400,
                },

                Subtitle1 = new Subtitle1()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "0.9rem",
                    FontWeight = 400,
                },

                Subtitle2 = new Subtitle2()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "0.9rem",
                    FontWeight = 400,
                },

                Button = new Button()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "0.9rem",
                    FontWeight = 1000,
                    LineHeight = 1.6,
                    LetterSpacing = "0.2em"
                },

                Caption = new Caption()
                {
                    FontFamily = new[] { "RobotoCondensed", "Roboto", "Helvetica", "Arial" },
                    FontSize = "0.8rem",
                }
            };
        }
    }
}
