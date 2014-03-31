(function () {
    var JQLiteAddCssHook, JQLiteAddCssHookExtension, JQLiteAddTransition, JQLiteAddTransitionExtension, Transform, camelize, checkTransform3dSupport, cssEase, cssFunc, cssHooks, div, eventNames, getProperties, getTransition, getVendorPropertyName, isChrome, isObject, propertyMap, registerCssHook, support, toMS, transitionEnd, trim, uncamel, unit, ___bind;
    cssHooks = {};
    propertyMap = {};
    support = {};
    getVendorPropertyName = function (prop) {
        var i, prefixes, prop_, vendorProp;
        if (prop in div.style) {
            return prop;
        }
        prefixes = [
          'Moz',
          'Webkit',
          'O',
          'ms'
        ];
        prop_ = prop.charAt(0).toUpperCase() + prop.substr(1);
        if (prop in div.style) {
            return prop;
        }
        i = 0;
        while (i < prefixes.length) {
            vendorProp = prefixes[i] + prop_;
            if (vendorProp in div.style) {
                return vendorProp;
            }
            ++i;
        }
    };
    checkTransform3dSupport = function () {
        div.style[support.transform] = '';
        div.style[support.transform] = 'rotateY(90deg)';
        return div.style[support.transform] !== '';
    };
    div = document.createElement('div');
    support = {};
    isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
    support.transition = getVendorPropertyName('transition');
    support.transitionDelay = getVendorPropertyName('transitionDelay');
    support.transform = getVendorPropertyName('transform');
    support.transformOrigin = getVendorPropertyName('transformOrigin');
    support.transform3d = checkTransform3dSupport();
    eventNames = {
        transition: 'transitionend',
        MozTransition: 'transitionend',
        OTransition: 'oTransitionEnd',
        WebkitTransition: 'webkitTransitionEnd',
        msTransition: 'MSTransitionEnd'
    };
    transitionEnd = support.transitionEnd = eventNames[support.transition] || null;
    cssEase = {
        '_default': 'ease',
        'in': 'ease-in',
        'out': 'ease-out',
        'in-out': 'ease-in-out',
        'snap': 'cubic-bezier(0,1,.5,1)',
        'easeOutCubic': 'cubic-bezier(.215,.61,.355,1)',
        'easeInOutCubic': 'cubic-bezier(.645,.045,.355,1)',
        'easeInCirc': 'cubic-bezier(.6,.04,.98,.335)',
        'easeOutCirc': 'cubic-bezier(.075,.82,.165,1)',
        'easeInOutCirc': 'cubic-bezier(.785,.135,.15,.86)',
        'easeInExpo': 'cubic-bezier(.95,.05,.795,.035)',
        'easeOutExpo': 'cubic-bezier(.19,1,.22,1)',
        'easeInOutExpo': 'cubic-bezier(1,0,0,1)',
        'easeInQuad': 'cubic-bezier(.55,.085,.68,.53)',
        'easeOutQuad': 'cubic-bezier(.25,.46,.45,.94)',
        'easeInOutQuad': 'cubic-bezier(.455,.03,.515,.955)',
        'easeInQuart': 'cubic-bezier(.895,.03,.685,.22)',
        'easeOutQuart': 'cubic-bezier(.165,.84,.44,1)',
        'easeInOutQuart': 'cubic-bezier(.77,0,.175,1)',
        'easeInQuint': 'cubic-bezier(.755,.05,.855,.06)',
        'easeOutQuint': 'cubic-bezier(.23,1,.32,1)',
        'easeInOutQuint': 'cubic-bezier(.86,0,.07,1)',
        'easeInSine': 'cubic-bezier(.47,0,.745,.715)',
        'easeOutSine': 'cubic-bezier(.39,.575,.565,1)',
        'easeInOutSine': 'cubic-bezier(.445,.05,.55,.95)',
        'easeInBack': 'cubic-bezier(.6,-.28,.735,.045)',
        'easeOutBack': 'cubic-bezier(.175, .885,.32,1.275)',
        'easeInOutBack': 'cubic-bezier(.68,-.55,.265,1.55)'
    };
    cssHooks['transit:transform'] = {
        get: function (elem) {
            return elem.data('transform') || new Transform();
        },
        set: function (elem, v) {
            var el, value, _i, _len;
            value = v;
            if (!(value instanceof Transform)) {
                value = new Transform(value);
            }
            for (_i = 0, _len = elem.length; _i < _len; _i++) {
                el = elem[_i];
                if (support.transform === 'WebkitTransform' && !isChrome) {
                    el.style[support.transform] = value.toString(true);
                } else {
                    el.style[support.transform] = value.toString();
                }
            }
            return elem.data('transform', value);
        }
    };
    registerCssHook = function (prop, isPixels) {
        propertyMap[prop] = support.transform;
        return cssHooks[prop] = {
            get: function (elem) {
                var t;
                t = elem.css('transit:transform');
                return t.get(prop);
            },
            set: function (elem, value) {
                var t;
                t = elem.css('transit:transform');
                t.setFromString(prop, value);
                return elem.css({ 'transit:transform': t });
            }
        };
    };
    registerCssHook('scale');
    registerCssHook('translate');
    registerCssHook('rotate');
    registerCssHook('rotateX');
    registerCssHook('rotateY');
    registerCssHook('rotate3d');
    registerCssHook('perspective');
    registerCssHook('skewX');
    registerCssHook('skewY');
    registerCssHook('x', true);
    registerCssHook('y', true);
    Transform = function () {
        function Transform(str) {
            if (typeof str === 'string') {
                this.parse(str);
            }
            return this;
        }
        Transform.prototype.setter = {
            rotate: function (theta) {
                return this.rotate = unit(theta, 'deg');
            },
            rotateX: function (theta) {
                return this.rotateX = unit(theta, 'deg');
            },
            rotateY: function (theta) {
                return this.rotateY = unit(theta, 'deg');
            },
            scale: function (x, y) {
                if (y === undefined) {
                    y = x;
                }
                return this.scale = x + ',' + y;
            },
            skewX: function (x) {
                return this.skewX = unit(x, 'deg');
            },
            skewY: function (y) {
                return this.skewY = unit(y, 'deg');
            },
            perspective: function (dist) {
                return this.perspective = unit(dist, 'px');
            },
            x: function (x) {
                return this.set('translate', x, null);
            },
            y: function (y) {
                return this.set('translate', null, y);
            },
            translate: function (x, y) {
                if (this._translateX === undefined) {
                    this._translateX = 0;
                }
                if (this._translateY === undefined) {
                    this._translateY = 0;
                }
                if (x !== null && x !== undefined) {
                    this._translateX = unit(x, 'px');
                }
                if (y !== null && y !== undefined) {
                    this._translateY = unit(y, 'px');
                }
                return this.translate = this._translateX + ',' + this._translateY;
            }
        };
        Transform.prototype.getter = {
            x: function () {
                return this._translateX || 0;
            },
            y: function () {
                return this._translateY || 0;
            },
            scale: function () {
                var s;
                s = (this.scale || '1,1').split(',');
                if (s[0]) {
                    s[0] = parseFloat(s[0]);
                }
                if (s[1]) {
                    s[1] = parseFloat(s[1]);
                }
                if (s[0] === s[1]) {
                    return s[0];
                } else {
                    return s;
                }
            },
            rotate3d: function () {
                var i, s;
                s = (this.rotate3d || '0,0,0,0deg').split(',');
                i = 0;
                while (i <= 3) {
                    if (s[i]) {
                        s[i] = parseFloat(s[i]);
                    }
                    ++i;
                }
                if (s[3]) {
                    s[3] = unit(s[3], 'deg');
                }
                return s;
            }
        };
        Transform.prototype.set = function (prop) {
            var args;
            args = Array.prototype.slice.apply(arguments, [1]);
            if (this.setter[prop]) {
                return this.setter[prop].apply(this, args);
            } else {
                return this[prop] = args.join(',');
            }
        };
        Transform.prototype.get = function (prop) {
            if (this.getter[prop]) {
                return this.getter[prop].apply(this);
            } else {
                return this[prop] || 0;
            }
        };
        Transform.prototype.setFromString = function (prop, val) {
            var args;
            args = typeof val === 'string' ? val.split(',') : val.constructor === Array ? val : [val];
            args.unshift(prop);
            return Transform.prototype.set.apply(this, args);
        };
        Transform.prototype.parse = function (str) {
            var self;
            self = this;
            return str.replace(/([a-zA-Z0-9]+)\((.*?)\)/g, function (x, prop, val) {
                return self.setFromString(prop, val);
            });
        };
        Transform.prototype.toString = function (use3d) {
            var i, re;
            re = [];
            for (i in this) {
                if (this.hasOwnProperty(i)) {
                    if (!support.transform3d && (i === 'rotateX' || i === 'rotateY' || i === 'perspective' || i === 'transformOrigin')) {
                        continue;
                    }
                    if (i[0] !== '_') {
                        if (use3d && i === 'scale') {
                            re.push(i + '3d(' + this[i] + ',1)');
                        } else if (use3d && i === 'translate') {
                            re.push(i + '3d(' + this[i] + ',0)');
                        } else {
                            re.push(i + '(' + this[i] + ')');
                        }
                    }
                }
            }
            return re.join(' ');
        };
        return Transform;
    }();
    isObject = function (value) {
        return value != null && typeof value === 'object';
    };
    trim = function (str) {
        return str.replace(/^\s+|\s+$/g, '');
    };
    camelize = function (str) {
        return trim(str).replace(/[-_\s]+(.)?/g, function (match, c) {
            if (c) {
                return c.toUpperCase();
            } else {
                return '';
            }
        });
    };
    uncamel = function (str) {
        return str.replace(/([A-Z])/g, function (letter) {
            return '-' + letter.toLowerCase();
        });
    };
    unit = function (i, units) {
        if (typeof i === 'string' && !i.match(/^[\-0-9\.]+$/)) {
            return i;
        } else {
            return '' + i + units;
        }
    };
    toMS = function (duration) {
        return unit(duration, 'ms');
    };
    getProperties = function (props) {
        var key, re;
        re = [];
        for (key in props) {
            key = camelize(key);
            key = propertyMap[key] || key;
            key = uncamel(key);
            if (re.indexOf(key) === -1) {
                re.push(key);
            }
        }
        return re;
    };
    getTransition = function (properties, duration, easing, delay) {
        var attribs, i, name, props, transitions, _i, _ref;
        props = getProperties(properties);
        if (cssEase[easing]) {
            easing = cssEase[easing];
        }
        attribs = '' + toMS(duration) + ' ' + easing;
        if (parseInt(delay, 10) > 0) {
            attribs += ' ' + toMS(delay);
        }
        transitions = [];
        for (i = _i = 0, _ref = props.length - 1; 0 <= _ref ? _i <= _ref : _i >= _ref; i = 0 <= _ref ? ++_i : --_i) {
            name = props[i];
            transitions.push(name + ' ' + attribs);
        }
        return transitions.join(', ');
    };
    JQLiteAddTransition = function (element, properties, duration, easing, callback) {
        var calback, callOrQueue, delay, i, oldTransitions, queue, run, self, transitionValue, work;
        self = element;
        delay = 0;
        queue = true;
        calback = void 0;
        if (typeof duration === 'function') {
            callback = duration;
            duration = void 0;
        }
        if (typeof easing === 'function') {
            callback = easing;
            easing = void 0;
        }
        if (properties.easing != null) {
            easing = properties.easing;
            delete properties.easing;
        }
        if (properties.duration != null) {
            duration = properties.duration;
            delete properties.duration;
        }
        if (properties.complete != null) {
            callback = properties.complete;
            delete properties.complete;
        }
        if (properties.queue != null) {
            queue = properties.queue;
            delete properties.queue;
        }
        if (properties.delay != null) {
            delay = properties.delay;
            delete properties.delay;
        }
        if (duration == null) {
            duration = 400;
        }
        if (easing == null) {
            easing = 'ease';
        }
        duration = toMS(duration);
        transitionValue = getTransition(properties, duration, easing, delay);
        work = support.transition;
        i = work ? parseInt(duration, 10) + parseInt(delay, 10) : 0;
        callOrQueue = function (fn, callback) {
            var queueFn;
            if (fn != null) {
                self.animationQueue.push({
                    runFn: fn,
                    callbackFn: callback
                });
            }
            if (!self.animating && self.animationQueue != null) {
                self.animating = true;
                queueFn = self.animationQueue.shift();
                if (queueFn) {
                    queueFn.runFn.call(self, function () {
                        self.animating = false;
                        return callOrQueue();
                    }, queueFn.callbackFn);
                } else {
                    self.animating = false;
                    self.animationQueue = [];
                }
            }
        };
        if (i === 0) {
            self.css(properties);
            if (callback != null) {
                callback.apply(self);
            }
            return self;
        }
        oldTransitions = {};
        run = function (next, endCallback) {
            var endTransitionCallback;
            endTransitionCallback = function () {
                if (!self.animationQueue.length) {
                    if (i > 0) {
                        self.css(support.transition, oldTransitions[this] || null);
                    }
                }
                if (typeof next === 'function') {
                    next();
                }
                if (typeof endCallback === 'function') {
                    return endCallback.apply(self);
                }
            };
            window.setTimeout(endTransitionCallback, i);
            if (i > 0) {
                self.css(support.transition, transitionValue);
            }
            return self.css(properties);
        };
        window.setTimeout(function () {
            return callOrQueue(run, callback);
        }, 0);
        return self;
    };
    cssFunc = angular.element.prototype.css;
    ___bind = function (fn, me) {
        return function () {
            return fn.apply(me, arguments);
        };
    };
    JQLiteAddCssHook = function (arg1, arg2) {
        var fn, hook, key, val;
        hook = void 0;
        val = void 0;
        fn = ___bind(cssFunc, this);
        if (isObject(arg1)) {
            for (key in arg1) {
                hook = cssHooks[key];
                val = void 0;
                if (hook && 'set' in hook) {
                    val = hook.set(this, arg1[key]);
                }
                if (val === void 0) {
                    val = fn(key, arg1[key]);
                }
            }
        } else if (arg2 != null) {
            hook = cssHooks[arg1];
            if (hook && 'set' in hook) {
                val = hook.set(this, arg2);
            }
            if (val === void 0) {
                return fn(arg1, arg2);
            }
        } else {
            if (this.length) {
                hook = cssHooks[arg1];
                if (hook && 'get' in hook) {
                    val = hook.get(this);
                }
                if (val === void 0) {
                    return fn(arg1, arg2);
                }
            }
        }
        return val;
    };
    JQLiteAddTransitionExtension = function (properties, duration, easing, callback) {
        var elementCount, elements, i, length;
        if (!this.animationQueue) {
            this.animationQueue = [];
            this.animating = false;
            this.bound = false;
        }
        length = this.length;
        elements = {};
        elementCount = 0;
        i = 0;
        while (i < this.length) {
            if (typeof this[i].style === 'undefined') {
                --length;
            } else {
                elements[elementCount++] = this[i];
            }
            delete this[i];
            i++;
        }
        this.length = length;
        i = 0;
        while (i < length) {
            this[i] = elements[i];
            i++;
        }
        if (!length) {
            return this;
        }
        return JQLiteAddTransition(this, properties, duration, easing, callback);
    };
    JQLiteAddCssHookExtension = function (propertyName, value) {
        var elementCount, elements, i, length;
        length = this.length;
        elements = {};
        elementCount = 0;
        i = 0;
        while (i < this.length) {
            if (typeof this[i].style === 'undefined') {
                --length;
            } else {
                elements[elementCount++] = this[i];
            }
            delete this[i];
            i++;
        }
        this.length = length;
        i = 0;
        while (i < length) {
            this[i] = elements[i];
            i++;
        }
        if (!length) {
            return this;
        }
        return JQLiteAddCssHook.call(this, propertyName, value);
    };
    angular.element.prototype['transition'] = JQLiteAddTransitionExtension;
    angular.element.prototype['css'] = JQLiteAddCssHookExtension;
}.call(this));