// jquery.multi-select.js
// by mySociety
// https://github.com/mysociety/jquery-multi-select

;(function($) {

    "use strict";

    var totalCheckbox = 0;

  var pluginName = "multiSelect",
    defaults = {
      'containerHTML': '<div class="multi-select-container">',
      'menuHTML': '<div class="multi-select-menu">',
        'buttonHTML': '<span class="multi-select-button" id="lblSelectedDepartment">',
      'buttonLabelHTML': '<span id="lblSelectedDepartmentValue" style="display: none;">',
      'menuItemsHTML': '<div class="multi-select-menuitems">',
      'menuItemHTML': '<label class="multi-select-menuitem">',
      'menuItemDivHTML': ' <div class="checkbox checkbox-success" style="padding-left: 25px;">',
      'presetsHTML': '<div class="multi-select-presets">',
      'presetsDivHTML': '<div class="radio radio-success" style="padding-left: 25px;">',
      'modalHTML': undefined,
      'activeClass': 'multi-select-container--open',
      'noneText': '-- Select Channel --',
      'allText': undefined,
      'presets': undefined,
      'positionedMenuClass': 'multi-select-container--positioned',
      'positionMenuWithin': undefined,
      'viewportBottomGutter': 20,
      'menuMinHeight': 200
    };

  /**
   * @constructor
   */
  function multiSelect(element, options) {
    this.element = element;
    this.$element = $(element);
    this.settings = $.extend( {}, defaults, options );
    this._defaults = defaults;
    this._name = pluginName;
    this._totalCheckbox = totalCheckbox;
    this.init();
  }

  function arraysAreEqual(array1, array2) {
    if ( array1.length != array2.length ){
      return false;
    }

    array1.sort();
    array2.sort();

    for ( var i = 0; i < array1.length; i++ ){
      if ( array1[i] !== array2[i] ){
        return false;
      }
    }

    return true;
  }

  $.extend(multiSelect.prototype, {

    init: function () { 
      this.checkSuitableInput();
      this.findLabels();
      this.constructContainer();
      this.constructButton();
      this.constructMenu();
      this.constructModal();

      this.setUpBodyClickListener();
      this.setUpLabelsClickListener();

      this.$element.hide();
    },

    checkSuitableInput: function(text) {
      if ( this.$element.is('select[multiple]') === false ) {
        throw new Error('$.multiSelect only works on <select multiple> elements');
      }
    },

    findLabels: function() {
      this.$labels = $('label[for="' + this.$element.attr('id') + '"]');
    },

    constructContainer: function() {
      this.$container = $(this.settings['containerHTML']);
      this.$element.data('multi-select-container', this.$container);
      this.$container.insertAfter(this.$element);
    },

    constructButton: function () {
      var _this = this;
      this.$button = $(this.settings['buttonHTML']);
      this.$labelButton = $(this.settings['buttonLabelHTML']).appendTo(this.$container);;

      //this.$labelButton.attr({
      //      'role': 'button',
      //      'aria-haspopup': 'true',
      //      'tabindex': 0,
      //      'aria-label': this.$labels.eq(0).text()
      //  });

      this.$button.attr({
        'role': 'button',
        'aria-haspopup': 'true',
        'tabindex': 0,
        'aria-label': this.$labels.eq(0).text()
      })
      .on('keydown.multiselect', function(e) {
        var key = e.which;
        var returnKey = 13;
        var escapeKey = 27;
        var spaceKey = 32;
        var downArrow = 40;
        if ((key === returnKey) || (key === spaceKey)) {
          e.preventDefault();
          _this.$button.click();
        } else if (key === downArrow) {
          e.preventDefault();
          _this.menuShow();
          var group = _this.$presets || _this.$menuItems;
          group.children(":first").focus();
        } else if (key === escapeKey) {
          _this.menuHide();
        }
      }).on('click.multiselect', function(e) {
        _this.menuToggle();
      })
      .appendTo(this.$container);

      this.$element.on('change.multiselect', function () {
          _this.updateButtonContents(); 
      }); 

      this.updateButtonContents();
    },

    updateButtonContents: function() {
      var _this = this;
      var options = [];
        var selected = [];
      //var options= [];
      //var selected = [];
  
      this.$element.children('option').each(function () {
         // debugger;
          var text = /** @type string */ ($(this).text());
          var key = /** @type string */ ($(this).val());
       
          options.push({ key: $.trim(key), value: $.trim(text) });
          if ($(this).is(':selected')) {
              selected.push({ key: $.trim(key), value: $.trim(text) });
        }
      }); 
    
      var presetValue = this.getPreset();
      if (presetValue !== "") {
          //debugger;
          selected.unshift({ key: presetValue, value: presetValue });
      }

      this.$button.empty();
      this.$labelButton.empty();
        
      if (Object.keys(selected).length == 0) {
        this.$button.text( this.settings['noneText'] );
      } else if ((Object.keys(selected).length === Object.keys(options).length) && this.settings['allText']) {
        this.$button.text( this.settings['allText'] );
      } else {
          var selectedChannel = selected.map(function (x) {  
              return x.value;
          }).join(', ');
          //debugger;
          var selectedChannelKey= selected.map(function (x) {
              return x.key;
          }).join(', ');
          this.$button.text(selectedChannel);
          this.$labelButton.text(selectedChannelKey);
      }
    },

    constructMenu: function() {
      var _this = this;

      this.$menu = $(this.settings['menuHTML']);
      this.$menu.attr({
        'role': 'menu'
      }).on('keyup.multiselect', function(e){
        var key = e.which;
        var escapeKey = 27;
        if (key === escapeKey) {
          _this.menuHide();
          _this.$button.focus();
        }
      })
      .appendTo(this.$container);

      this.constructMenuItems();

      if ( this.settings['presets'] ) {
        this.constructPresets();
      }
    },

    constructMenuItems: function() {
      var _this = this;
     // debugger;
      this.$menuItems = $(this.settings['menuItemsHTML']);
      this.$menu.append(this.$menuItems);

      this.$element.on('change.multiselect', function (e, internal) {
        // Don't need to update the menu items if this
          // change event was fired by our tickbox handler.
         // debugger;
        if(internal !== true){
          //_this.updateMenuItems();
        }
      });

      this.updateMenuItems();
    },

    updateMenuItems: function () {
        //debugger;
      var _this = this;
      this.$menuItems.empty();

      this.$element.children('option').each(function (optionIndex, option) {
         // debugger;
        var $item = _this.constructMenuItem($(option), optionIndex);
        _this.$menuItems.append($item);
      });
    },

    upDown: function(type, e) {
    var key = e.which;
    var upArrow = 38;
    var downArrow = 40;

    if (key === upArrow) {
      e.preventDefault();
      var prev = $(e.currentTarget).prev();
      if (prev.length) {
        prev.focus();
      } else if (this.$presets && type === 'menuitem') {
        this.$presets.children(':last').focus();
      } else {
        this.$button.focus();
      }
    } else if (key === downArrow) {
      e.preventDefault();
      var next = $(e.currentTarget).next();
      if (next.length || type === 'menuitem') {
        next.focus();
      } else {
        this.$menuItems.children(':first').focus();
      }
    }
  },

    constructPresets: function() {
      var _this = this;
      this.$presets = $(this.settings['presetsHTML']);
      this.$menu.prepend(this.$presets);

      $.each(this.settings['presets'], function(i, preset){
        var uniqueId = _this.$element.attr('name') + '_preset_' + i;
        var $radioDiv = $(_this.settings['presetsDivHTML']);

          var $item = $(_this.settings['menuItemHTML'])
              .attr({
                  'for': uniqueId,
                  'role': 'menuitem'
              })
              .text(' ' + preset.name)
              .on('keydown.multiselect', _this.upDown.bind(_this, 'preset'));
          // .appendTo(_this.$presets);
          totalCheckbox ++;

          var $input = $('<input>')
              .attr({
                  'type': 'radio',
                  'name': _this.$element.attr('name') + '_presets',
                  'id': uniqueId
              });
        //  .appendTo(_this.$presets);
       
         $input.appendTo($radioDiv);
         $item.appendTo($radioDiv);
         $radioDiv.appendTo(_this.$presets);

         $input.on('change.multiselect', function () {
             //debugger;
          //_this.$element.val(preset.options);
          _this.$element.trigger('change');
        });
      });

      this.$element.on('change.multiselect', function () {
        _this.updatePresets();
      });

      this.updatePresets();
    },

    updatePresets: function() {
      var _this = this;

      $.each(this.settings['presets'], function (i, preset) { 
        var uniqueId = _this.$element.attr('name') + '_preset_' + i;
        var $input = _this.$presets.find('#' + uniqueId);
          //debugger;
          var a = _this.$element.val();

          //if ( arraysAreEqual(preset.options || [], _this.$element.val() || []) ){ 
          //  $input.prop('checked', true);
          //} else {
          //  $input.prop('checked', false);
          //}
      });
    },

    getPreset: function() {
        var _this = this;
        var name = "";
        $.each(this.settings['presets'], function (i, preset) {
            var uniqueId = _this.$element.attr('name') + '_preset_' + i; 
            var isChecked = $("input[id='" + uniqueId + "']").is(':checked');
            if (isChecked) {
                name = preset.name;
                return name;
            }
            return name;
        });
        return name;
    },

    constructMenuItem: function($option, optionIndex) {
        var _this = this;
        //debugger;
        var uniqueId = this.$element.attr('name') + '_' + optionIndex;

        var $item = $(this.settings['menuItemHTML'])
          .attr({
              'for': uniqueId,
              'role': 'menuitem'
          })
          .on('keydown.multiselect', this.upDown.bind(this, 'menuitem'))
          .text(' ' + $option.text());

        var $input = $('<input>')
            .attr({
                'type': 'checkbox',
                'id': uniqueId,
                'value': $option.val()
            });
        //.prependTo($item);
        var $div = $(this.settings['menuItemDivHTML']);
        $input.appendTo($div);
        $item.appendTo($div);

        if ($option.is(':disabled')) {
            $input.attr('disabled', 'disabled');
        }
        if ($option.is(':selected')) {
            $input.prop('checked', 'checked');
        }

        $input.on('change.multiselect', function () {
          //  debugger;
            if ($(this).prop('checked')) {
                $option.prop('selected', true);
            } else {
                $option.prop('selected', false);
            }

            if ($(this).val() === "All001" && $(this).prop('checked')) {
                $(".multi-select-menuitems input:checkbox").prop('checked', true);

                _this.$element.children('option').each(function (oindex, o) { 
                    $(o).prop('selected', true);
                });
                _this.updateButtonContents();

            } else if ($(this).val() === "All001" && !$(this).is(':selected')) {
                $(".multi-select-menuitems input:checkbox").prop('checked', false);
                _this.$element.children('option').each(function (oindex, o) {
                    $(o).prop('selected', false);
                });
                _this.updateButtonContents();
            }

            if ($(this).val() !== "All001" && !$(this).is(':selected')) {
                //$(".multi-select-menuitems input:checkbox").prop('checked', false); 
               
                _this.$element.children('option').each(function (oindex, o) {

                    if ($(o).is(':selected') && $(o).val() === "0") {
                        //debugger; 
                        var uniqueId = _this.$element.attr('name') + '_' + oindex;
                        $("input[id='" + uniqueId + "']").prop('checked', false); 
                      
                        $(o).prop('selected', false);
                        return false;
                    } 
                }); 
            }

            // .prop() on its own doesn't generate a change event.
            // Other plugins might want to do stuff onChange.
            //debugger;
            $option.trigger('change', [true]);
        });

        return $div;
    },

    constructModal: function() {
      var _this = this;

      if (this.settings['modalHTML']) {
        this.$modal = $(this.settings['modalHTML']);
        this.$modal.on('click.multiselect', function(){
          _this.menuHide();
        })
        this.$modal.insertBefore(this.$menu);
      }
    },

    setUpBodyClickListener: function() {
      var _this = this;

      // Hide the $menu when you click outside of it.
      $('html').on('click.multiselect', function(){
        _this.menuHide();
      });

      // Stop click events from inside the $button or $menu from
      // bubbling up to the body and closing the menu!
      this.$container.on('click.multiselect', function(e){
        e.stopPropagation();
      });
    },

    setUpLabelsClickListener: function() {
      var _this = this;
      this.$labels.on('click.multiselect', function(e) {
        e.preventDefault();
        e.stopPropagation();
        _this.menuToggle();
      });
    },

    menuShow: function() {
      $('html').trigger('click.multiselect'); // Close any other open menus
      this.$container.addClass(this.settings['activeClass']);

      if ( this.settings['positionMenuWithin'] && this.settings['positionMenuWithin'] instanceof $ ) {
        var menuLeftEdge = this.$menu.offset().left + this.$menu.outerWidth();
        var withinLeftEdge = this.settings['positionMenuWithin'].offset().left +
          this.settings['positionMenuWithin'].outerWidth();

        if ( menuLeftEdge > withinLeftEdge ) {
          this.$menu.css( 'width', (withinLeftEdge - this.$menu.offset().left) );
          this.$container.addClass(this.settings['positionedMenuClass']);
        }
      }

      var menuBottom = this.$menu.offset().top + this.$menu.outerHeight();
      var viewportBottom = $(window).scrollTop() + $(window).height();
      if ( menuBottom > viewportBottom - this.settings['viewportBottomGutter'] ) {
        this.$menu.css({
          'maxHeight': Math.max(
            viewportBottom - this.settings['viewportBottomGutter'] - this.$menu.offset().top,
            this.settings['menuMinHeight']
          ),
          'overflow': 'scroll'
        });
      } else {
        this.$menu.css({
          'maxHeight': '',
          'overflow': ''
        });
      }
    },

    menuHide: function() {
      this.$container.removeClass(this.settings['activeClass']);
      this.$container.removeClass(this.settings['positionedMenuClass']);
      this.$menu.css('width', 'auto');
    },

    menuToggle: function() {
      if ( this.$container.hasClass(this.settings['activeClass']) ) {
        this.menuHide();
      } else {
        this.menuShow();
      }
    }

  });

  $.fn[ pluginName ] = function(options) {
    return this.each(function() {
      if ( !$.data(this, "plugin_" + pluginName) ) {
        $.data(this, "plugin_" + pluginName,
          new multiSelect(this, options) );
      }
    });
  };

})(jQuery);
