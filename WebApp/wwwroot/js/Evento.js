var vm = new Vue({
    el: "#vEvento",
    data: {
        loading: false,
        editDto: { Id: "", Titulo: "", Decricao: "", DataEvento: "", Status: true },
        editControlePresencaEventoDto: { Id: "", Convidado: "", Controle: "", EventoId: "" },
        params: {
            visible: true
        }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            var formid = $('form')[1].id;

            //triggered when modal is about to be shown
            $('#mdUpload').on('show.bs.modal', function (e) {

                //get data-id attribute of the clicked element
                var id = $(e.relatedTarget).data('id');

                $("input[name='eventoId']").val(id);
            });

            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            if (formid === "formControlePresencaEvento") {

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

                var alunoConvidadoEvento = $("input:radio[name=alunoConvidadoEvento]");
                alunoConvidadoEvento.on("change", function () {
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

                    if ($(this).val() == "A") {
                        self.params.visible = true;
                    } else if ($(this).val() == "C") {
                        self.params.visible = false;
                    }
                });

                $("#formControlePresencaEvento").validate({
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
            if (formid === "formEditControlePresencaEvento") {

                $("#formEditControlePresencaEvento").validate({
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
            if (formid === "formEditEvento") {

                var $dtEvento = $("#dtEvento");
                $dtEvento.mask('00/00/0000', { reverse: false });

                $("#formEditEvento").validate({
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

            if (formid === "formEvento") {

                var $dtEvento = $("#dtEvento");
                $dtEvento.mask('00/00/0000', { reverse: false });

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
                                    title: 'Evento',
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

                $("#formEvento").validate({
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
        DeleteEvento: function (id) {
            var url = "Evento/Delete/" + id;
            $("#deleteEventoHref").prop("href", url);
        },
        EditEvento: function (id) {
            var self = this;

            axios.get("Evento/GetEventoById/?id=" + id).then(result => {
                self.editDto.Id = result.data.id;
                self.editDto.Titulo = result.data.titulo;
                self.editDto.Descricao = result.data.descricao;
                self.editDto.DataEvento = result.data.dataEvento;
                self.editDto.Status = result.data.status;
                

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        DeleteControlePresencaEvento: function (id, eventoId) {
            var url = "../Evento/DeleteControlePresenca?id=" + id+"&eventoId="+eventoId;
            $("#deleteControlePresencaEventoHref").prop("href", url);
        },
        EditControlePresencaEvento: function (id, eventoId) {
            var self = this;

            axios.get("../ControlePresenca/GetControlePresencaById/?id=" + id).then(result => {
                if (result.data.justificativa === "") {
                    self.params.visible = true;
                }
                else {
                    self.params.visible = false;
                }


                self.editControlePresencaEventoDto.Id = result.data.id;
                self.editControlePresencaEventoDto.EventoId = eventoId;
                self.editControlePresencaEventoDto.AlunoId = result.data.alunoId;
                self.editControlePresencaEventoDto.NomeAluno = result.data.nomeAluno;
                self.editControlePresencaEventoDto.Convidado = result.data.justificativa;
                self.editControlePresencaEventoDto.Controle = result.data.controle;
            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteEventoId"]').attr('value', id);
        $('#mdDeleteEvento').modal('show');
        vm.DeleteEvento(id)
    },
    EditModal: function (id) {
        $('input[name="editEventoId"]').attr('value', id);
        $('#mdEditEvento').modal('show');
        vm.EditEvento(id)
    },
    DeleteControlePresencaEventoModal: function (id, eventoId) {
        $('input[name="deleteControlePresencaEventoId"]').attr('value', id);
        $('#mdDeleteControlePresencaEvento').modal('show');
        vm.DeleteControlePresencaEvento(id, eventoId)
    },
    EditControlePresencaEventoModal: function (id, eventoId) {
        $('input[name="editControlePresencaEventoId"]').attr('value', id);
        $('#mdEditControlePresencaEvento').modal('show');
        vm.EditControlePresencaEvento(id, eventoId)
    }
};