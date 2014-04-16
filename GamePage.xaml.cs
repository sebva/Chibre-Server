using Chibre_Server.Common;
using Chibre_Server.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// Pour en savoir plus sur le modèle d'élément Page de base, consultez la page http://go.microsoft.com/fwlink/?LinkId=234237

namespace Chibre_Server
{
    /// <summary>
    /// Page de base qui inclut des caractéristiques communes à la plupart des applications.
    /// </summary>
    public sealed partial class GamePage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private static GamePage latestInstance;
        private ResourceLoader loader = new ResourceLoader();

        /// <summary>
        /// Cela peut être remplacé par un modèle d'affichage fortement typé.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper est utilisé sur chaque page pour faciliter la navigation et 
        /// gestion de la durée de vie des processus
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public static Windows.UI.Core.CoreDispatcher LatestDispatcher
        {
            get { return latestInstance.Dispatcher;  }
        }


        public GamePage()
        {
            latestInstance = this;
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            ScoreCanvas.Loaded += DrawScoreBoard;
        }

        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="sender">
        /// La source de l'événement ; en général <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Données d'événement qui fournissent le paramètre de navigation transmis à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page et
        /// un dictionnaire d'état conservé par cette page durant une session
        /// antérieure. L'état n'aura pas la valeur Null lors de la première visite de la page.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            GameEngine gameEngine = GameEngine.Instance;
            defaultViewModel["Table"] = gameEngine.Table;
            defaultViewModel["GameEngine"] = gameEngine;
            gameEngine.Team1.Score.PropertyChanged += Score_PropertyChanged;
            gameEngine.Team2.Score.PropertyChanged += Score_PropertyChanged;
        }

        void Score_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Score")
                DrawScoreBoard();
        }

        #region ScoreBoard

        private Brush lineBrush = new SolidColorBrush(Colors.Red);
        private Brush stickBrush = new SolidColorBrush(Colors.White);
        private Brush oneBrush = new SolidColorBrush(Colors.White);
        private List<Line> constantLines = null;
        private const double marginLeftRight = 50;
        private const double marginUpDown = 70;
        private const double lineStroke = 3;
        private const double oneFontSize = 40;
        private const double teamFontSize = 18;
        private const double innerSticksMargin = 10;
        private const double outerSticksMargin = 18;
        private const double stickHeight = 40;
        private const double stickStroke = 3;
        private const double diagonalStickOverlap = 5;

        private void DrawScoreBoard()
        {
            if (constantLines == null)
                InitializeConstantLines();

            ScoreCanvas.Children.Clear();
            double height = ScoreCanvas.ActualHeight;
            double width = ScoreCanvas.ActualWidth;
            if (double.IsNaN(height)) // Not yet ready
                return;

            GameEngine gameEngine = GameEngine.Instance;

            #region Team1
            Score score1 = gameEngine.Team1.Score;

            DrawScoreSticks(score1.Twenty, marginUpDown / 2.0, false);
            DrawScoreSticks(score1.Fifty, height / 4.0, false);
            DrawScoreSticks(score1.Hundred, height / 2.0 - marginUpDown / 2.0 - teamFontSize, false);

            TextBlock ones1 = new TextBlock()
            {
                Text = Convert.ToString(score1.One),
                FontSize = oneFontSize,
                Foreground = oneBrush,
                RenderTransform = new RotateTransform() { Angle=180 },
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            ScoreCanvas.Children.Add(ones1);
            Canvas.SetLeft(ones1, marginLeftRight);
            Canvas.SetTop(ones1, height / 4.0 - oneFontSize * 2.0 / 3.0);

            TextBlock team1 = new TextBlock()
            {
                Text = loader.GetString("Team1Members"),
                FontSize = teamFontSize,
                Foreground = oneBrush,
                RenderTransform = new RotateTransform() { Angle = 180 },
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            ScoreCanvas.Children.Add(team1);
            team1.Measure(new Size(1000, 1000));
            Canvas.SetLeft(team1, width / 2.0 - team1.DesiredSize.Width / 2.0);
            Canvas.SetTop(team1, height / 2.0 - teamFontSize * 1.5);

            #endregion

            #region Team2
            Score score2 = gameEngine.Team2.Score;

            DrawScoreSticks(score2.Twenty, height - marginUpDown / 2.0, true);
            DrawScoreSticks(score2.Fifty, height * 0.75, true);
            DrawScoreSticks(score2.Hundred, height / 2.0 + marginUpDown / 2.0 + teamFontSize, true);

            TextBlock ones2 = new TextBlock()
            {
                Text = Convert.ToString(score2.One),
                FontSize = oneFontSize,
                Foreground = oneBrush
            };
            ScoreCanvas.Children.Add(ones2);
            ones2.Measure(new Size(1000, 1000));
            Canvas.SetLeft(ones2, width - marginLeftRight - ones2.DesiredSize.Width);
            Canvas.SetTop(ones2, height * 0.75 - oneFontSize / 2.0);

            TextBlock team2 = new TextBlock()
            {
                Text = loader.GetString("Team2Members"),
                FontSize = teamFontSize,
                Foreground = oneBrush
            };
            ScoreCanvas.Children.Add(team2);
            team2.Measure(new Size(1000, 1000));
            Canvas.SetLeft(team2, width / 2.0 - team2.DesiredSize.Width / 2.0);
            Canvas.SetTop(team2, height / 2.0 + teamFontSize * 0.25);

            #endregion
            
            foreach (Line line in constantLines)
                ScoreCanvas.Children.Add(line);
        }

        private void DrawScoreSticks(int amount, double y, bool leftToRight)
        {
            List<Line> lines = new List<Line>();

            double inversion = leftToRight ? 1.0 : -1.0;
            double x = leftToRight ? marginLeftRight : ScoreCanvas.ActualWidth - marginLeftRight;
            double xInit = x;
            for (int i = 1; i <= amount; i++)
            {
                if (i % 5 == 0)
                {
                    lines.Add(new Line()
                    {
                        X1 = xInit - diagonalStickOverlap * inversion,
                        Y1 = y + (stickHeight / 2.0 * inversion),
                        X2 = x - (innerSticksMargin * inversion) + diagonalStickOverlap * inversion,
                        Y2 = y - (stickHeight / 2.0 * inversion),
                        Stroke = stickBrush,
                        StrokeThickness = stickStroke
                    });
                    x += (outerSticksMargin - innerSticksMargin) * inversion;
                    xInit = x;
                }
                else
                {
                    lines.Add(new Line()
                    {
                        X1 = x,
                        Y1 = y - stickHeight / 2.0,
                        X2 = x,
                        Y2 = y + stickHeight / 2.0,
                        Stroke = stickBrush,
                        StrokeThickness = stickStroke
                    });
                    x += (innerSticksMargin * inversion);
                }
            }
                
            foreach(Line line in lines)
                ScoreCanvas.Children.Add(line);
        }

        private void InitializeConstantLines()
        {
            double height = ScoreCanvas.ActualHeight;
            double width = ScoreCanvas.ActualWidth;
            constantLines = new List<Line>(7);


            // Middle line
            constantLines.Add(new Line()
            {
                X1 = 0,
                Y1 = height / 2.0,
                X2 = width,
                Y2 = height / 2.0,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
            // Team 1 upper Z line
            constantLines.Add(new Line()
            {
                X1 = marginLeftRight,
                Y1 = marginUpDown,
                X2 = width - marginLeftRight,
                Y2 = marginUpDown,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
            // Team 1 diagonal Z line
            constantLines.Add(new Line()
            {
                X1 = width - marginLeftRight,
                Y1 = marginUpDown,
                X2 = marginLeftRight,
                Y2 = height / 2.0 - marginUpDown - teamFontSize,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
            // Team 1 lower Z line
            constantLines.Add(new Line()
            {
                X1 = marginLeftRight,
                Y1 = height / 2.0 - marginUpDown - teamFontSize,
                X2 = width - marginLeftRight,
                Y2 = height / 2.0 - marginUpDown - teamFontSize,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
            // Team 2 upper Z line
            constantLines.Add(new Line()
            {
                X1 = marginLeftRight,
                Y1 = height / 2.0 + marginUpDown + teamFontSize,
                X2 = width - marginLeftRight,
                Y2 = height / 2.0 + marginUpDown + teamFontSize,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
            // Team 2 diagonal Z line
            constantLines.Add(new Line()
            {
                X1 = width - marginLeftRight,
                Y1 = height / 2.0 + marginUpDown + teamFontSize,
                X2 = marginLeftRight,
                Y2 = height - marginUpDown,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
            // Team 2 lower Z line
            constantLines.Add(new Line()
            {
                X1 = marginLeftRight,
                Y1 = height - marginUpDown,
                X2 = width - marginLeftRight,
                Y2 = height - marginUpDown,
                Stroke = lineBrush,
                StrokeThickness = lineStroke
            });
        }

        private void DrawScoreBoard(object sender, RoutedEventArgs e)
        {
            DrawScoreBoard();
        }

        #endregion

        /// <summary>
        /// Conserve l'état associé à cette page en cas de suspension de l'application ou de la
        /// suppression de la page du cache de navigation.  Les valeurs doivent être conformes aux
        /// exigences en matière de sérialisation de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">La source de l'événement ; en général <see cref="NavigationHelper"/></param>
        /// <param name="e">Données d'événement qui fournissent un dictionnaire vide à remplir à l'aide de
        /// état sérialisable.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Inscription de NavigationHelper

        /// Les méthodes fournies dans cette section sont utilisées simplement pour permettre
        /// NavigationHelper pour répondre aux méthodes de navigation de la page.
        /// 
        /// La logique spécifique à la page doit être placée dans les gestionnaires d'événements pour  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// et <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Le paramètre de navigation est disponible dans la méthode LoadState 
        /// en plus de l'état de page conservé durant une session antérieure.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
