//TODO Add Script TO bundle!

function $SimpleSlider($, sliderID, sliderValID, defaultVal, max, min, onChanged, options) {
    if (typeof options === 'undefined') options = {}
    if (typeof options.valShowerFunc === 'undefined')
        options.valShowerFunc = function (val) { return val }

    var sliderWrapper = $('#' + sliderID)
    var sliderCont = document.createElement('div')
    sliderCont.id = sliderID + 'slider'
    sliderWrapper.append(sliderCont)
    sliderCont = $("#" + sliderID + 'slider')

    var sliderValCont = $("#" + sliderValID)

    UpdateSliderValShowing(defaultVal)
    sliderCont.slider({
        max: max,
        min: min,
        value: ApplyInvert(defaultVal),
        slide: function (event, ui) { UpdateSliderValShowing(ApplyInvert(ui.value)) },
        stop: function (event, ui) { onChanged (Val()) }
    })

    function UpdateSliderValShowing(v) {
        sliderValCont.html(options.valShowerFunc(v))
    }

    function ApplyInvert(v) {
        return options.inverted ? max - v + min : v
    }

    function Val() {
        return ApplyInvert(sliderCont.slider("option", "value")) 
    }

    this.GetCurrentVal = Val

    this.SetVal = function (val) {
        if (val > max || (val < min))
            return;
        sliderCont.slider('value', ApplyInvert(val) );
        UpdateSliderValShowing(val)
        onChanged(Val())
    }
    this.ValUp = function () {
        this.SetVal(Val() + 1)
    }
    this.ValDown = function () {
        this.SetVal(Val() - 1)
    }
}