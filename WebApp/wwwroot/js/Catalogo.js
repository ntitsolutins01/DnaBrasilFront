var vm = new Vue({
    el: "#vCatalogo",
    data: {
        loading: false,
        editDto: { Id: "", Nome: "", Status: true }
    },
    mounted: function () {
        var self = this;
        // Carousel
        (function ($) {

            'use strict';

            if ($.isFunction($.fn['owlCarousel'])) {

                $(function () {
                    $('[data-plugin-carousel]').each(function () {
                        var $this = $(this),
                            opts = {};

                        var pluginOptions = $this.data('plugin-options');
                        if (pluginOptions)
                            opts = pluginOptions;

                        $this.themePluginCarousel(opts);
                    });
                });

            }

            //triggered when modal is about to be shown
            $('#mdDetalheCurso').on('show.bs.modal', function (e) {

                //get data-id attribute of the clicked element
                var id = $(e.relatedTarget).data('id');

                $("input[name='cursoId']").val(id);

                if (id === "") {
                    Site.Notification("Catálogo de Curso", "Por favor selecione um curso", "warning");
                }

                var url = "../Curso/GetDetalheCurso";

                axios.get(url, {
                    params: {
                        id: id
                    }
                }).then(result => {
                    $("#mdDetalheCurso").find(".modal-body").html(result.data);

                }).catch(error => {
                    Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                });




            });

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