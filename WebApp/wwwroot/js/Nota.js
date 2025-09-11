var vm = new Vue({
    el: "#vNota",
    data: {
        loading: false,
        editDto: { Id: "", NotaPrimeiroBimestre: "", NotaSegundoBimestre: "", NotaTerceiroBimestre: "", notaQuartoBimestre: "", Status: true }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            //mascara dos inputs
            var notaPrimeiroBimestre = $("#notaPrimeiroBimestre");
            notaPrimeiroBimestre.mask('00,00', { reverse: false });

            var notaSegundoBimestre = $("#notaSegundoBimestre");
            notaSegundoBimestre.mask('00,00', { reverse: false });

            var notaTerceiroBimestre = $("#notaTerceiroBimestre");
            notaTerceiroBimestre.mask('00,00', { reverse: false });

            var notaQuartoBimestre = $("#notaQuartoBimestre");
            notaQuartoBimestre.mask('00,00', { reverse: false });

            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            var formid = $('form')[1].id;

            if (formid === "formEditNota") {

                $("#formEditNota").validate({
                    rules:
                    {
                        notaPrimeiroBimestre:
                        {
                            range: [0.00, 10]
                        },
                        notaSegundoBimestre:
                        {
                            range: [0.00, 10]
                        },
                        notaTerceiroBimestre:
                        {
                            range: [0.00, 10]
                        },
                        notaQuartoBimestre:
                        {
                            range: [0.00, 10]
                        }
                    },
                    messages: {
                        "notaPrimeiroBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        },
                        "notaSegundoBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        },
                        "notaTerceiroBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        },
                        "notaQuartoBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        }
                    },
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

            if (formid === "formNota") {
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
                                    title: 'Nota',
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

                $("#formNota").validate({
                    rules:
                    {
                        notaPrimeiroBimestre:
                        {
                            range: [0.00, 10]
                        },
                        notaSegundoBimestre:
                        {
                            range: [0.00, 10]
                        },
                        notaTerceiroBimestre:
                        {
                            range: [0.00, 10]
                        },
                        notaQuartoBimestre:
                        {
                            range: [0.00, 10]
                        }
                    },
                    messages: {
                        "notaPrimeiroBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        },
                        "notaSegundoBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        },
                        "notaTerceiroBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        },
                        "notaQuartoBimestre": {
                            range: "Nota deve ser entre 00,00 à 10."
                        }
                    },
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
        DeleteNota: function (id) {
            var url = "Nota/Delete/" + id;
            $("#deleteNotaHref").prop("href", url);
        },
        EditNota: function (id) {
            var self = this;

            axios.get("Nota/GetNotaById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.NotaPrimeiroBimestre = result.data.primeiroBimestre;
                self.editDto.NotaSegundoBimestre = result.data.segundoBimestre;
                self.editDto.NotaTerceiroBimestre = result.data.terceiroBimestre;
                self.editDto.NotaQuartoBimestre = result.data.quartoBimestre;
                self.editDto.Status = result.data.status;
                

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteNotaId"]').attr('value', id);
        $('#mdDeleteNota').modal('show');
        vm.DeleteNota(id)
    },
    EditModal: function (id) {
        $('input[name="editNotaId"]').attr('value', id);
        $('#mdEditNota').modal('show');
        vm.EditNota(id)
    }
};