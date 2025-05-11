
var vm = new Vue({
    el: "#formTipoLaudo",
    data: {
        loading: false,
        editDto: { Id: "", Nome: "", Descricao: "", Status: true, Idade: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            var formid = $('form')[1].id;

            if (formid === "formEditTiposLaudo") {

                $("#formEditTiposLaudo").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }

            if (formid === "formTipoLaudo") {

                $("#formTipoLaudo").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }
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
        },
        DeleteTiposLaudo: function (id) {
            var url = "TiposLaudo/Delete/" + id;
            $("#deleteTiposLaudoHref").prop("href", url);
        },
        EditTiposLaudo: function (id) {
            var self = this;

            axios.get("TiposLaudo/GetTiposLaudoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Nome = result.data.nome;
                self.editDto.Descricao = result.data.descricao;
                self.editDto.Status = result.data.status;
                self.editDto.Idade = result.data.idadeMinima;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="TiposLaudoId"]').attr('value', id);
        $('#mdDeleteTiposLaudo').modal('show');
        vm.DeleteTiposLaudo(id)
    },
    EditModal: function (id) {
        $('input[name="TiposLaudoId"]').attr('value', id);
        $('#mdEditTiposLaudo').modal('show');
        vm.EditTiposLaudo(id)
    }
};