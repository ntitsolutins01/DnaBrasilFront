var vm = new Vue({
    el: "#vCategoria",
    data: {
        loading: false,
        editDto: {
            Id: "", Categoria: "", Nome: "", Decricao: "",
            IdadeInicial: "", IdadeFinal: "", Status: true
        },
        params: {
            visible: true
        }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            var formid = $('form')[1].id;

            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            if (formid === "formEditCategoria") {

                $("#formEditCategoria").validate({
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

            if (formid === "formCategoria") {

                $("#formCategoria").validate({
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
        DeleteCategoria: function (id) {
            var url = "Categoria/Delete/" + id;
            $("#deleteCategoriaHref").prop("href", url);
        },
        EditCategoria: function (id) {
            var self = this;

            self.editDto = {
                Id: "",
                Codigo: "",
                Nome: "",
                Descricao: "",
                idadeInicial: "",
                IdadeFim: "",
                Status: true
            };

            axios.get("Categoria/GetCategoriaById/?id=" + id).then(result => {
                //console.log('Dados retornados:', result.data);

                self.$nextTick(() => {
                    self.editDto = {
                        Id: result.data.id,
                        Nome: result.data.nome,
                        Codigo: result.data.codigo,
                        IdadeInicial: result.data.idadeInicial,
                        IdadeFinal: result.data.idadeFinal,
                        Descricao: result.data.descricao,
                        Status: result.data.status
                    };
                });

            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteCategoriaId"]').attr('value', id);
        $('#mdDeleteCategoria').modal('show');
        vm.DeleteCategoria(id)
    },
    EditModal: function (id) {
        $('input[name="editCategoriaId"]').attr('value', id);
        $('#mdEditCategoria').modal('show');
        vm.EditCategoria(id)
    }
};