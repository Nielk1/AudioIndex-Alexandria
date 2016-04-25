// Nielk1's rewrite
(function () {
    var slice = [].slice;

    (function($, window) {
        "use strict";
        var BootstrapCheckedListGroup;
        BootstrapCheckedListGroup = (function () {
            function BootstrapCheckedListGroup(element, options) {
                if (options == null) {
                    options = {};
                }
                this.$widget = $(element);
                this.options = $.extend({}, $.fn.bootstrapCheckedListGroup.defaults, {
                    color: this.$widget.data('color'),
                    style: this.$widget.data('style') == "button" ? "btn-" : null
                    //state: this.$element.is(":checked"),
                    //size: this.$element.data("size"),
                    //animate: this.$element.data("animate"),
                    //disabled: this.$element.is(":disabled"),
                    //readonly: this.$element.is("[readonly]"),
                    //indeterminate: this.$element.data("indeterminate"),
                    //inverse: this.$element.data("inverse"),
                    //radioAllOff: this.$element.data("radio-all-off"),
                    //onColor: this.$element.data("on-color"),
                    //offColor: this.$element.data("off-color"),
                    //onText: this.$element.data("on-text"),
                    //offText: this.$element.data("off-text"),
                    //labelText: this.$element.data("label-text"),
                    //handleWidth: this.$element.data("handle-width"),
                    //labelWidth: this.$element.data("label-width"),
                    //baseClass: this.$element.data("base-class"),
                    //wrapperClass: this.$element.data("wrapper-class")
                }, options);
                this.prevOptions = {};
                this.$checkbox = $('<input type="checkbox" class="hidden" />');
                //this.color = (this.$widget.data('color') ? this.$widget.data('color') : "primary");
                //this.style = (this.$widget.data('style') == "button" ? "btn-" : "list-group-item-");
                this.settings = {
                    on: {
                        icon: 'glyphicon glyphicon-check'
                    },
                    off: {
                        icon: 'glyphicon glyphicon-unchecked'
                    }
                };

                this.$widget.css('cursor', 'pointer')
                this.$widget.append(this.$checkbox);

                // Event Handlers
                this.$widget.on('click', (function(_this) {
                    return function () {
                        _this.$checkbox.prop('checked', !_this.$checkbox.is(':checked'));
                        _this.$checkbox.triggerHandler('change');
                        _this.updateDisplay();
                    }
                })(this));
                this.$checkbox.on('change', (function(_this) {
                    return function () {
                        _this.updateDisplay();
                    }
                })(this));





                // Actions
                this.updateDisplay = (function (_this) {
                    return function () {
                        var isChecked = _this.$checkbox.is(':checked');

                        // Set the button's state
                        _this.$widget.data('state', (isChecked) ? "on" : "off");

                        // Set the button's icon
                        _this.$widget.find('.state-icon')
                            .removeClass()
                            .addClass('state-icon ' + _this.settings[_this.$widget.data('state')].icon);

                        // Update the button's color
                        if (isChecked) {
                            _this.$widget.addClass(_this.options.style + _this.options.color + ' active');
                        } else {
                            _this.$widget.removeClass(_this.options.style + _this.options.color + ' active');
                        }
                    }
                })(this);

                this._init();
                //this._elementHandlers();
            }

            BootstrapCheckedListGroup.prototype._constructor = BootstrapCheckedListGroup;

            BootstrapCheckedListGroup.prototype.setPrevOptions = function () {
                return this.prevOptions = $.extend(true, {}, this.options);
            };

            BootstrapCheckedListGroup.prototype._init = function () {
                var init, initInterval;
                init = (function (_this) {
                    return function () {
                        _this.setPrevOptions();

                        if (_this.$widget.data('checked') == true) {
                            _this.$checkbox.prop('checked', !_this.$checkbox.is(':checked'));
                        }

                        _this.updateDisplay();

                        // Inject the icon if applicable
                        if (_this.$widget.find('.state-icon').length == 0) {
                            _this.$widget.prepend('<span class="state-icon ' + _this.settings[_this.$widget.data('state')].icon + '"></span>');
                        }
                    };
                })(this);
                //if (this.$wrapper.is(":visible")) {
                //    return init();
                //}
                //return initInterval = window.setInterval((function (_this) {
                //    return function () {
                //        if (_this.$wrapper.is(":visible")) {
                //            init();
                //            return window.clearInterval(initInterval);
                //        }
                //    };
                //})(this), 50);
                return init();
            };

            return BootstrapCheckedListGroup;
        })();

        BootstrapCheckedListGroup.prototype.state = function (value) {
            if (typeof value === "undefined") {
                return this.$checkbox.is(':checked');
            }
            if (this.options.disabled || this.options.readonly) {
                return this.$element;
            }
            value = !!value;
            this.$element.prop("checked", value);//.trigger("change.bootstrapSwitch");
            return this.$element;
        };

        $.fn.bootstrapCheckedListGroup = function () {
            var args, option, ret;
            option = arguments[0], args = 2 <= arguments.length ? slice.call(arguments, 1) : [];
            ret = this;
            this.each(function () {
                var $this, data;
                $this = $(this);
                data = $this.data("bootstrap-checked-list-group");
                if (!data) {
                    $this.data("bootstrap-checked-list-group", data = new BootstrapCheckedListGroup(this, option));
                }
                if (typeof option === "string") {
                    return ret = data[option].apply(data, args);
                }
            });
            return ret;
        };
        $.fn.bootstrapCheckedListGroup.Constructor = BootstrapCheckedListGroup;
        return $.fn.bootstrapCheckedListGroup.defaults = {
            color: "primary",
            style: "list-group-item-"
            //state: true
            //size: null,
            //animate: true,
            //disabled: false,
            //readonly: false,
            //indeterminate: false,
            //inverse: false,
            //radioAllOff: false,
            //onColor: "primary",
            //offColor: "default",
            //onText: "ON",
            //offText: "OFF",
            //labelText: "&nbsp;",
            //handleWidth: "auto",
            //labelWidth: "auto",
            //baseClass: "bootstrap-switch",
            //wrapperClass: "wrapper",
            //onInit: function () { },
            //onSwitchChange: function () { }
        };
    })(window.jQuery, window);
}).call(this);