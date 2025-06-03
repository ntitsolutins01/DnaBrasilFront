var vm = new Vue({
    el: "#vAlunoCurso",
    data: {
        loading: false,
        editDto: { Id: "", Progresso: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

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

            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            var formid = $('form')[0].id;

            if (formid === "formEditAlunoCurso") {

                $("#formEditAlunoCurso ").validate({
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

            if (formid === "formAlunoCurso") {

                $("#formAlunoCurso").validate({
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

            $("#ddlEstado").change(function () {

                self.ShowLoad(true, "pFiltro");

                var sigla = $("#ddlEstado").val();

                var url = "../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

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
                                title: 'Usuario',
                                text: data,
                                type: 'warning'
                            });
                        }
                    });

                self.ShowLoad(false, "pFiltro");
            });

            //clique de escolha do select
            $("#ddlMunicipio").change(function () {

                self.ShowLoad(true, "pFiltro");

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

                self.ShowLoad(false, "pFiltro");
            });

            $("#ddlLocalidade").change(function () {

                self.ShowLoad(true, "pFiltro");

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
                                title: 'Aluno',
                                text: 'Alunos não encontrados.',
                                type: 'warning'
                            });
                        }
                    });

                self.ShowLoad(false, "pFiltro");
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
        },
        DeleteAlunoCurso: function (id) {
            var url = "AlunoCurso/Delete/" + id;
            $("#deleteAlunoCursoHref").prop("href", url);
        },
        EditAlunoCurso: function (id) {
            var self = this;

            axios.get("AlunoCurso/GetAlunoCursoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Progresso = result.data.progresso;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        AlunoCurso: function (id) {
            var self = this;

            axios.get("AlunoCurso/GetAlunoCursoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Progresso = result.data.progresso;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteAlunoCursoId"]').attr('value', id);
        $('#mdDeleteAlunoCurso').modal('show');
        vm.DeleteAlunoCurso(id)
    },
    EditModal: function (id) {
        $('input[name="editAlunoCursoId"]').attr('value', id);
        $('#mdEditAlunoCurso').modal('show');
        vm.EditAlunoCurso(id)
    },
    AlunoCursoModal: function (id) {
        $('input[name="certificadoId"]').attr('value', id);
        $('#mdAlunoCurso').modal('show');
        vm.AlunoCurso(id);
    }
};