(function ($) {
    'use strict';

    $(function () {
        // 操作提示自动淡出，仍可通过关闭按钮手动关闭。
        window.setTimeout(function () {
            $('.alert-dismissible').fadeOut(400);
        }, 5000);

        // 为带 data-confirm 的表单提供二次确认能力。
        $('form[data-confirm]').on('submit', function (event) {
            var message = $(this).data('confirm');
            if (message && !window.confirm(message)) {
                event.preventDefault();
            }
        });
    });
})(jQuery);
