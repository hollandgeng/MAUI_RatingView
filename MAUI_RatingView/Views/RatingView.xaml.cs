using MAUI_RatingView.DataClass;
using MAUI_RatingView.Enum;
using MAUI_RatingView.Event;
using MAUI_RatingView.Material;

namespace MAUI_RatingView;

public partial class RatingView : HorizontalStackLayout
{
    readonly WeakEventManager _ratingValueChanged = new();

    public event EventHandler<RatingValueEventArgs> RatingValueChanged
    {
        add => _ratingValueChanged.AddEventHandler(value);
        remove => _ratingValueChanged.RemoveEventHandler(value);
    }

    private void OnValueChanged()
    {
        _ratingValueChanged?.HandleEvent(this, new RatingValueEventArgs(CurrentValue), nameof(RatingValueChanged));
    }

    public RatingView()
	{
		InitializeComponent();

        UpdateLayout();
        UpdateItemSize();
    }

    private const int minRating = 1;
    private List<FontImageSource> fontImageSources;

    public static readonly BindableProperty TotalRatingProperty
        = BindableProperty.Create(nameof(TotalRating), typeof(int), typeof(RatingView), 5, BindingMode.OneWay, propertyChanged: OnTotalRatingChanged);

    public int TotalRating
    {
        get => (int)GetValue(TotalRatingProperty);
        set => SetValue(TotalRatingProperty, value);
    }

    public static readonly BindableProperty SelectedColorProperty
        = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(RatingView),
            Application.Current.Resources.TryGetValue("ThemeColor", out var themeColor) ? (Color)themeColor : Colors.Green,
            propertyChanged: OnColorChanged);

    public static readonly BindableProperty CurrentValueProperty
        = BindableProperty.Create(nameof(CurrentValue), typeof(int), typeof(RatingView), 1, BindingMode.OneWay, propertyChanged: OnValueChanged);

    public int CurrentValue
    {
        get => (int)GetValue(CurrentValueProperty);
        set => SetValue(CurrentValueProperty, value);
    }

    public static readonly BindableProperty ItemSizeProperty
        = BindableProperty.Create(nameof(ItemSize), typeof(int), typeof(RatingView), 50, BindingMode.OneWay, propertyChanged: OnItemSizeChanged);

    public int ItemSize
    {
        get => (int)GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }


    private static void OnItemSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is RatingView ratingView)
        {
            ratingView.UpdateItemSize();
        }
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is RatingView ratingView)
        {
            if (oldValue.Equals(newValue)) return;

            ratingView.UpdateValue();
            ratingView.OnValueChanged();
        }
    }

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly BindableProperty UnselectedColorProperty
       = BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(RatingView), Colors.Gray, propertyChanged: OnColorChanged);

    public Color UnselectedColor
    {
        get => (Color)GetValue(UnselectedColorProperty);
        set => SetValue(UnselectedColorProperty, value);
    }

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is RatingView ratingView)
        {
            ratingView.ChangeColor();
        }
    }

    private static void OnTotalRatingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is RatingView ratingView)
        {
            ratingView.UpdateLayout();
        }
    }

    private void UpdateLayout()
    {
        if (TotalRating <= 0) TotalRating = minRating;

        int currentRating = this.Children.Count;
        int countToAmend = Math.Abs(currentRating - TotalRating);

        if (currentRating > TotalRating)
        {
            for (int i = 0; i < countToAmend; i++)
            {
                //remove the last item               
                Image ratingView = (Image)this.Children.LastOrDefault();
                RemoveImageTapBehavior(ratingView);
                this.Children.Remove(ratingView);
                RemoveLastFontImageSource();
            }
        }
        else if (currentRating < TotalRating)
        {
            for (int i = 0; i < countToAmend; i++)
            {
                Image image = GenerateReviewItem(RatingItemState.Empty);
                SetupRatingTapBehavior(image);
                this.Children.Add(image);
            }
        }
        else
        {
            return;
        }
    }

    private void ChangeColor()
    {
        if (fontImageSources == null || fontImageSources.Count == 0) return;

        foreach (RatingItem item in fontImageSources)
        {
            item.Color = item.State switch
            {
                RatingItemState.Fill => SelectedColor,
                RatingItemState.Half => SelectedColor,
                RatingItemState.Empty => UnselectedColor,
                _ => UnselectedColor
            };
        }
    }

    private void UpdateValue()
    {
        if (CurrentValue <= 0) CurrentValue = 1;

        if (CurrentValue >= TotalRating)
        {
            foreach (Image item in this.Children)
            {
                FontImageSource source = item.Source as FontImageSource;
                source.Color = SelectedColor;
                source.Glyph = MaterialIconOutlined.Star;
            }
        }
        else
        {
            int itemCount = 1;
            foreach (Image item in this.Children)
            {
                RatingItem source = item.Source as RatingItem;

                if (itemCount <= CurrentValue)
                {
                    source.Color = SelectedColor;
                    source.Glyph = MaterialIconOutlined.Star;
                    source.State = RatingItemState.Fill;
                }
                else
                {
                    source.Color = UnselectedColor;
                    source.Glyph = MaterialIconOutlined.Grade;
                    source.State = RatingItemState.Empty;
                }

                itemCount++;
            }
        }
    }

    private void UpdateItemSize()
    {
        foreach (Image item in this.Children)
        {
            item.HeightRequest = ItemSize;
        }
    }

    private Image GenerateReviewItem(RatingItemState state)
    {
        Image ratingIcon = new Image();

        RatingItem fontImageSource = new RatingItem()
        {
            FontFamily = nameof(MaterialIconOutlined),
            State = state
        };

        switch (state)
        {
            case RatingItemState.Empty:
                fontImageSource.Glyph = MaterialIconOutlined.Grade;
                fontImageSource.Color = UnselectedColor;
                break;
            case RatingItemState.Fill:
                fontImageSource.Glyph = MaterialIconOutlined.Star;
                fontImageSource.Color = SelectedColor;
                break;
            case RatingItemState.Half:
                fontImageSource.Glyph = MaterialIconOutlined.Star_half;
                fontImageSource.Color = SelectedColor;
                break;
            default:
                break;
        }

        AddFontImageSource(fontImageSource);
        ratingIcon.Source = fontImageSource;

        return ratingIcon;
    }

    private void SetupRatingTapBehavior(Image target)
    {
        TapGestureRecognizer tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnRatingTapped;

        target.GestureRecognizers.Add(tapGesture);
    }

    private void RemoveImageTapBehavior(Image target)
    {
        TapGestureRecognizer tapGesture = (TapGestureRecognizer)target.GestureRecognizers.FirstOrDefault();
        tapGesture.Tapped -= OnRatingTapped;

        target.GestureRecognizers.Remove(tapGesture);
    }

    private void OnRatingTapped(object sender, TappedEventArgs e)
    {
        Image ratingView = sender as Image;
        int index = this.Children.IndexOf(ratingView);
        CurrentValue = index + 1;
    }

    private void AddFontImageSource(FontImageSource source)
    {
        if (fontImageSources == null)
            fontImageSources = new List<FontImageSource>();

        fontImageSources.Add(source);
    }

    private void RemoveFontImageSource(FontImageSource source)
    {
        if (fontImageSources == null || fontImageSources.Count == 0) return;

        fontImageSources.Remove(source);
    }

    private void RemoveLastFontImageSource()
    {
        if (fontImageSources == null || fontImageSources.Count == 0) return;

        fontImageSources.RemoveAt(fontImageSources.Count - 1);
    }
}