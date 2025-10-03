var vm = new Vue({
    el: "#formEtapaEnsino",
    data: {
        loading: false,
        editDto: { Id: "", Nome: "", Status: true }
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

            if ($("#formEditEtapaEnsino").length > 0) {
                $("#formEditEtapaEnsino").validate({
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
        EditEtapaEnsino: function (id) {
            var self = this;
            axios.get("/EtapaEnsino/GetEtapaEnsinoById?id=" + id)
                .then(result => {
                    self.editDto.Id = result.data.id;
                    self.editDto.Nome = result.data.nome;
                    self.editDto.Status = result.data.status;
                })
                .catch(error => {
                    new PNotify({
                        title: 'Erro',
                        text: 'Não foi possível buscar os dados da Etapa de Ensino.',
                        type: 'error'
                    });
                    console.error("Erro ao buscar Etapa de Ensino:", error);
                });
        },
        DeleteEtapaEnsino: function (id) {
            var url = "/EtapaEnsino/Delete/" + id;
            $("#deleteEtapaEnsinoHref").prop("href", url);
        },
        onStatusChange: function(event) {
            this.editDto.Status = event.target.checked;
        }
    }
});

var crud = {
    EditModal: function (id) {
        $('input[name="Id"]').val(id);
        vm.EditEtapaEnsino(id);
        $('#mdEditEtapaEnsino').modal('show');
    },
    DeleteModal: function (id) {
        vm.DeleteEtapaEnsino(id);
        $('#mdDeleteEtapaEnsino').modal('show');
    }
};
