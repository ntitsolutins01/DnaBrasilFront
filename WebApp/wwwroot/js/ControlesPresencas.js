var vm = new Vue({
    el: "#vControlePresenca",
    data: {
        loading: false,
        editDto: { Id: "", Controle: "", Justificativa: "", Data: "", NomeAluno: "", MunicipioEstado: "", NomeLocalidade: "", AlunoId: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            var formid = $('form')[1].id;

            if (formid === "formPesquisarAluno") {

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

                //clique de escolha do select
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
                    var id = $("#ddlLocalidade").val();

                    var url = "../../Aluno/GetAlunosByLocalidade?id=" + id;
                    $.getJSON(url,
                    { id: id },
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
                                    text: "Aluno não encontrado.",
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlModalidade").change(function () {
                    var modalidadeId = $("#ddlModalidade").val();

                    var profissionalId = $("#ddlProfissional").val();

                    var url = "../Profissional/GetTurmasByModalidadeIdProfissionalId";

                    $.getJSON(url,
                        { modalidadeId: modalidadeId, profissionalId: profissionalId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Turma</option>';
                                $("#ddlTurma").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlTurma").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: "O Profissional logado não possui atividades e turmas cadastradas.",
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlTurma").change(function () {

                    var id = $("#ddlTurma").val();

                    if (id === "") {
                        Site.Notification("Profissional", "Por favor selecione uma turma", "warning");
                    }

                    var url = "../Atividade/GetAtividadeById";

                    var urlDataTable = "../Atividade/GetAtividadeAlunosByAtividadeId";

                    axios.get(url, {
                        params: {
                            id: id
                        }
                    }).then(result => {
                        $("#divAlunos").show();
                        self.editDto.Categoria = result.data.nomeCategoria;
                        self.editDto.Estrutura = result.data.nomeEstrutura;
                        self.editDto.DiasSemana = result.data.diasSemana;
                        self.editDto.Horario = result.data.hrInicial + " - " + result.data.hrFinal;

                        axios.get(urlDataTable, {
                            params: {
                                id: id
                            }
                        }).then(result => {
                            if (result.data.length > 0) {

                                self.editDto.Update = true;

                                $.each(result.data,
                                    function (i, item) {

                                        $('#alunoDataTable').DataTable().destroy();

                                        var table = $('#alunoDataTable').DataTable({
                                            columnDefs: [
                                                { "className": "text-center", "targets": "_all" }
                                            ]
                                        });

                                        table.row.add([item.alunoId.toString(), item.alunoId + " - " + item.nome,
                                        "<a style='color:#F44336' href='javascript:(crud.DeleteAluno(\"" + item.alunoId + "\"))'><i class='fa fa-trash'></i></a>"])
                                            .draw();

                                        self.params.alunos.push(item.alunoId.toString());

                                    });

                                $('input[name="arrAlunos"]').attr('value', self.params.alunos);
                            } else {

                                self.editDto.Update = false;
                            }
                        }).catch(error => {
                            Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                        });

                    }).catch(error => {
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    });
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
        DeleteControlePresenca: function (id) {
            var url = "ControlePresenca/Delete/" + id;
            $("#deleteControlePresencaHref").prop("href", url);
        },
        EditControlePresenca: function (id) {
            var self = this;

            axios.get("ControlePresenca/GetControlePresencaById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Controle = result.data.controle;
                self.editDto.Data = result.data.data;
                self.editDto.Justificativa = result.data.justificativa;
                self.editDto.NomeAluno = result.data.nomeAluno;
                self.editDto.MunicipioEstado = result.data.municipioEstado;
                self.editDto.NomeLocalidade = result.data.nomeLocalidade;
                self.editDto.AlunoId = result.data.alunoId;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteControlePresencaId"]').attr('value', id);
        $('#mdDeleteControlePresenca').modal('show');
        vm.DeleteControlePresenca(id)
    },
    EditModal: function (id) {
        $('input[name="editControlePresencaId"]').attr('value', id);
        $('#mdEditControlePresenca').modal('show');
        vm.EditControlePresenca(id)
    }
};