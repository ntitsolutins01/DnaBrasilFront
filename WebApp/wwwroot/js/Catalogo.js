var vm = new Vue({
    el: "#vCatalogo",
    data: {
        loading: false,
        editDto: { Id: "", Nome: "", Status: true }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            
        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            var self = this;

            self.isLoading = flag;
            $("#" + el).loadingOverlay({
                "startShowing": flag
            });
            self.loading = flag;

            if (!flag) {
                self.isLoading = flag;
                $("#" + el).removeClass("loading-overlay-showing");
                self.loading = flag;
            } else {
                self.isLoading = flag;
                $("#" + el).addClass("loading-overlay-showing");
                self.loading = flag;
            }
        }
    }
});

var crud = {
    //DeleteModal: function (id) {
    //    $('input[name="TipoParceriaId"]').attr('value', id);
    //    $('#mdDeleteTipoParceria').modal('show');
    //    vm.DeleteTipoParceria(id)
    //},
    //EditModal: function (id) {
    //    $('input[name="TipoParceriaId"]').attr('value', id);
    //    $('#mdEditTipoParceria').modal('show');
    //    vm.EditTipoParceria(id)
    //}
};