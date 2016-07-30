function $encodeparsToId(prefix, pars) {
    var output = prefix
    for (prop in pars) {
        if (pars.hasOwnProperty(prop))
            output += "." + prop + ":" + pars[prop]
    }
    return output
}

function $decodeIdToPars(prefixToCheck, input) {
    var pars = input.split(".")
    if (prefixToCheck !== pars[0])
        return null;
    pars.shift()
    var out = {}
    pars.forEach(function (e) {
        var kv = e.split(":")
        out[kv[0]] = kv[1]
    })
    return out;
}

function $clearfixJQ(el) {
    var elClone = el.cloneNode(true);
    el.parentNode.replaceChild(elClone, el);
    return elClone
}

function $draggable(el) {
    el.draggable = true;
    el.addEventListener(
        'dragstart',
        function (e) {
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('Text', this.id);
            this.classList.add('drag');
            return false;
        },
        false
    );
    el.addEventListener(
        'dragend',
        function (e) {
            this.classList.remove('drag');
            return false;
        },
        false
    );
}

function $droppable(el, onDrop) {
    el.addEventListener(
        'dragover',
        function (e) {
            e.preventDefault()
            e.stopPropagation()
            e.dataTransfer.dropEffect = 'move';
            this.classList.add('drag-over');
            return false;
        },
        false
    );
    el.addEventListener(
        'dragenter',
        function (e) {
            this.classList.add('drag-over');
            return false;
        },
        false
    );
    el.addEventListener(
        'dragleave',
        function (e) {
            this.classList.remove('drag-over');
            return false;
        },
        false
    );
    el.addEventListener('drop', OnDropHandler, false)

    function OnDropHandler (e) {
        // Stops some browsers from redirecting.
        e.preventDefault()
        e.stopPropagation()
        //alert("!")
        var binId = this.id;
        var item = document.getElementById(e.dataTransfer.getData('Text'));
        this.classList.remove('drag-over');
        //use this if you want manipulate DOM
        //var item = document.getElementById(e.dataTransfer.getData('Text'));
        //this.appendChild(item);
        onDrop(item.id, binId)
        return false;
    }
}
