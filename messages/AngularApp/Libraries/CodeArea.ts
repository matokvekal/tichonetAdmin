function CodeArea(codeAreaID) {
    //todo this objects is TOO HARDCODED, refactor..

    var $container = $('#' + codeAreaID + ' .container');
    var $backdrop = $('#' + codeAreaID + ' .textareabackdrop');
    var $highlights = $('#' + codeAreaID + ' .highlights');
    var $textarea = $('#' + codeAreaID + ' textarea');

    // yeah, browser sniffing sucks, but there are browser-specific quirks 
    // to handle that are not a matter of feature detection
    var ua = window.navigator.userAgent.toLowerCase();
    var isIE = !!ua.match(/msie|trident\/7|edge/);
    var isWinPhone = ua.indexOf('windows phone') !== -1;
    var isIOS = !isWinPhone && !!ua.match(/ipad|iphone|ipod/);

    function applyHighlights(text) {
        text = text
            .replace(/\n$/g, '\n\n')
            .replace(/(\{\|[^|].*?\|\})/g, '<mark class="codeopen">$&</mark>')
            .replace(/(\{\|\|.*?\|\})/g, '<mark class="codeclose">$&</mark>')
            .replace(/(\{[^|].*?\})/g, '<mark class="wc">$&</mark>')
            ;

        if (isIE) {
            // IE wraps whitespace differently in a div vs textarea, this fixes it
            text = text.replace(/ /g, ' <wbr>');
        }

        return text;
    }

    this.HandleInput = () => handleInput()

    this.Clear = () => $highlights.html("")

    function handleInput() {

        var text = $textarea.val();
        var highlightedText = applyHighlights(text);
        //alert(highlightedText)
        $highlights.html(highlightedText);
    }

    function handleScroll() {
        var scrollTop = $textarea.scrollTop();
        $backdrop.scrollTop(scrollTop);

        var scrollLeft = $textarea.scrollLeft();
        $backdrop.scrollLeft(scrollLeft);
    }

    function fixIOS() {
        // iOS adds 3px of (unremovable) padding to the left and right of a textarea, 
        // so adjust highlights div to match
        $highlights.css({
            'padding-left': '+=3px',
            'padding-right': '+=3px'
        });
    }

    function bindEvents() {
        $textarea.on({
            //'input': handleInput,
            'scroll': handleScroll
        });
    }

    if (isIOS) {
        fixIOS();
    }

    bindEvents();
    handleInput();
}

//var abv = new CodeArea("codearea")