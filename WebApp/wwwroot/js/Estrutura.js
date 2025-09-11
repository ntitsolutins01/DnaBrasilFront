var vm = new Vue({
    el: "#vEstrutura",
    data: {
        loading: false,
        editDto: { Id: "", Nome: "", Decricao: "", Status: true },
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

            //skin select
            var $select = $(".select2").select2({
                allowClear: true
            });

            $(".select2").each(function () {
                var $this = $(this),
                    opts = {};

                var pluginOptions = $this.data('plugin-options');
                if (pluginOptions)
                    opts = pluginOptions;

                $this.themePluginSelect2(opts);
            });

            /*
             * When you change the value the select via select2, it triggers
             * a 'change' event, but the jquery validation plugin
             * only re-validates on 'blur'*/

            $select.on('change', function () {
                $(this).trigger('blur');
            });

            if (formid === "formEditEstrutura") {

                $("#formEditEstrutura").validate({
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

            if (formid === "formEstrutura") {

                

                $("#ddlEstado").change(function () {
                    var sigla = $("#ddlEstado").val();

                    var url = "../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

                    var ddlSource = "#ddlMunicipio";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Municipio</option>';
                                $("#ddlMunicipio").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlMunicipio").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Estrutura',
                                    text: 'Municípios não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlMunicipio").change(function () {
                    var id = $("#ddlMunicipio").val();

                    var url = "../../Localidade/GetLocalidadeByMunicipio?id=" + id;

                    var ddlSource = "#ddlLocalidade";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Localidade</option>';
                                $("#ddlLocalidade").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlLocalidade").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Localidades',
                                    text: 'Localidades não encontradas.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlLocalidade").change(function () {
                    var id = $("#ddlLocalidade").val();

                    var url = "../../Aluno/GetAlunosByLocalidadeId?id=" + id;

                    var ddlSource = "#ddlAluno";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Aluno</option>';
                                $("#ddlAluno").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlAluno").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Alunos',
                                    text: 'Alunos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#formEstrutura").validate({
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
        DeleteEstrutura: function (id) {
            var url = "Estrutura/Delete/" + id;
            $("#deleteEstruturaHref").prop("href", url);
        },
        EditEstrutura: function (id) {
            var self = this;

            self.editDto = {
                Id: "",
                Nome: "",
                Descricao: "",
                Status: true
            };

            axios.get("Estrutura/GetEstruturaById/?id=" + id).then(result => {
                //console.log('Dados retornados:', result.data);

                self.$nextTick(() => {
                    self.editDto = {
                        Id: result.data.id,
                        Nome: result.data.nome,
                        Descricao: result.data.descricao,
                        Status: result.data.status
                    };
                });

                if (result.data.listLocalidades && result.data.listLocalidades.length > 0) {
                    var items = '<option value="">Selecionar a Localidade</option>';
                    $("#ddlLocalidade").empty();
                    $.each(result.data.listLocalidades,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlLocalidade").html(items);
                } else {
                    new PNotify({
                        title: 'Localidade',
                        text: 'Localidades não encontradas.',
                        type: 'warning'
                    });
                }
            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteEstruturaId"]').attr('value', id);
        $('#mdDeleteEstrutura').modal('show');
        vm.DeleteEstrutura(id)
    },
    EditModal: function (id) {
        $('input[name="editEstruturaId"]').attr('value', id);
        $('#mdEditEstrutura').modal('show');
        vm.EditEstrutura(id)
    }
};