using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Wpf_Bugs
{
    public class Vm :DependencyObject
    {
        private BitmapImage _bugs;
        private BitmapImage _food;
        private BitmapImage _poison;

        public static int Slider { get; set; }

        private TextBlock labels;


        //public Canvas Canva;
        public ObservableCollection<UIElement> CanvasChildren { get; } = new ObservableCollection<UIElement>();
        public ObservableCollection<UIElement> CanvasBugs { get; } = new ObservableCollection<UIElement>();

        private readonly Uri bugsUri = new Uri(@"Images\Bugs.png", UriKind.Relative);
        private readonly Uri foodUri = new Uri(@"Images\food.png", UriKind.Relative);
        private readonly Uri poisonUri = new Uri(@"Images\poison.png", UriKind.Relative);

        public ObservableCollection<int> Steps { get; } = new ObservableCollection<int>();
        
        public int Generation
        {
            get { return (int)GetValue(stepProperty); }
            set { SetValue(stepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty stepProperty =
            DependencyProperty.Register("Generation", typeof(int), typeof(Vm), new PropertyMetadata(0));



        public Vm()
        {
            Steps.Add(0);
            Slider = 100;
            _bugs = new BitmapImage(bugsUri);
            _food = new BitmapImage(foodUri);
            _poison = new BitmapImage(poisonUri);
            
            for (int i = 0; i < 100; i++)
            {
                Image t = new Image();
                t.Source = _bugs;
                CanvasChildren.Add(t);
                Canvas.SetLeft(CanvasChildren[i], 2000);
                Canvas.SetTop(CanvasChildren[i], 2000);
            }
            
            for (int i = 100; i < 200; i++)
            {
                Image t = new Image();
                t.Source = _food;
                CanvasChildren.Add(t);
                Canvas.SetLeft(CanvasChildren[i], 2000);
                Canvas.SetTop(CanvasChildren[i], 2000);
            }
            
            
            for (int i = 200; i < 300; i++)
            {
                Image t = new Image();
                t.Source = _poison;
                CanvasChildren.Add(t);
                Canvas.SetLeft(CanvasChildren[i], 2000);
                Canvas.SetTop(CanvasChildren[i], 2000);
            }

            for (int i = 300; i < 400; i++)
            {
                labels = new TextBlock();
                labels.FontSize = 12;
                labels.Foreground = Brushes.Red;
                Canvas.SetLeft(labels, 2000);
                Canvas.SetTop(labels, 2000);
                CanvasChildren.Add(labels);
            }
           
            World.Move += Mouve;
            World.ReloadWorld += ReloadWorld;
            World.Create();
        }

        private void ReloadWorld(object sender, ReloadEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                Generation = e.Generation;
                if (Steps.Count > 22)
                {
                    Steps.RemoveAt(22);
                }

                Steps.Insert(1,e.Step);
            }));
           
        }

        private void Mouve(object sender, MoveEventArgs e)
        {
            //if (!this.Dispatcher.)
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (e.Life > 0)
                    {
                        ((TextBlock) CanvasChildren[e.Id + 300]).Text = e.Life.ToString();
                        Canvas.SetLeft(CanvasChildren[e.Id + 300], ((e.X - 1) * 20) + 3);
                        Canvas.SetTop(CanvasChildren[e.Id + 300], ((e.Y - 1) * 20) + 3);
                    }

                    Canvas.SetLeft(CanvasChildren[e.Id], (e.X - 1) * 20);
                    Canvas.SetTop(CanvasChildren[e.Id], (e.Y - 1) * 20);
                    Steps[0] = e.Step;
                }));
            }
        }

     

        

    }

}


