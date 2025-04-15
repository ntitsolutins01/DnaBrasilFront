var vm = new Vue({
    el: "#vPesquisarAluno",
    data: {
        loading: false,
        editDto: { Id: "", FomentoId: "", Nome: "", Status: true, Email: "", Sexo: "", DtNascimento: "", MunicipioEstado: "", NomeLocalidade: "", Cpf: "", Image: "", Modalidades: "", Cep: "", Etinia: "", Deficiencia: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            // iosSwitcher
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

            }).apply(this, [jQuery]);

            var formid = $('form')[1].id;

            //triggered when modal is about to be shown
            $('#mdUpload').on('show.bs.modal', function (e) {

                //get data-id attribute of the clicked element
                var id = $(e.relatedTarget).data('id');

                $("input[name='alunoId']").val(id);
            });

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

                if ($.isFunction($.fn['tooltip'])) {
                    $('[data-toggle=tooltip],[rel=tooltip]').tooltip({ container: 'body' });
                }

                //clique de escolha do select Estado
                $("#ddlEstado").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var sigla = $("#ddlEstado").val();

                    // Limpa apenas municípios e localidades, mas mantém o fomento
                    $("#ddlMunicipio").empty();
                    $("#ddlLocalidade").empty();

                    if (!sigla) {
                        self.ShowLoad(false, "pFiltro");
                        return;
                    }

                    var url = "../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

                    $.getJSON(url, function (data) {
                        if (data.length > 0) {
                            var items = '';
                            $.each(data, function (i, row) {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            });
                            $("#ddlMunicipio").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Municípios',
                                text: 'Municípios não encontrados.',
                                type: 'warning'
                            });
                        }
                    });

                    self.ShowLoad(false, "pFiltro");
                });

                //clique de escolha do select Municipio
                $("#ddlMunicipio").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var id = $("#ddlMunicipio").val();

                    $("#ddlLocalidade").empty();

                    if (!id) {
                        self.ShowLoad(false, "pFiltro");
                        return;
                    }

                    var url = "../../Localidade/GetLocalidadeByMunicipio?id=" + id;

                    $.getJSON(url, function (data) {
                        if (data.length > 0) {
                            var items = '';
                            $.each(data, function (i, row) {
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

                $("#ddlFomento").change(function () {
                    var fomentoId = $(this).val();

                    if (!fomentoId) {
                        return;
                    }

                    self.ShowLoad(true, "pFiltro");

                    // busca o fomento
                    $.getJSON("../../Fomento/GetFomentoById", { id: fomentoId })
                        .done(function (f) {
                            // Define o estado diretamente da resposta do fomento
                            $("#ddlEstado").val(f.sigla).trigger('change');

                            // Aguarda um momento para que o evento change do estado seja processado
                            setTimeout(function () {
                                // Carrega os municípios do estado
                                $.getJSON("../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + f.sigla)
                                    .done(function (municipios) {
                                        // Limpa e preenche o dropdown de municípios
                                        var items = '';
                                        $.each(municipios, function (i, row) {
                                            items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                        });
                                        $("#ddlMunicipio").html(items);
                                        $("#ddlMunicipio").val(f.municipioId).trigger('change');

                                        // Aguarda um momento para que o evento change do município seja processado
                                        setTimeout(function () {
                                            // Carrega as localidades do município
                                            $.getJSON("../../Localidade/GetLocalidadeByMunicipio?id=" + f.municipioId)
                                                .done(function (localidades) {
                                                    // Limpa e preenche o dropdown de localidades
                                                    var localItems = '';
                                                    $.each(localidades, function (i, row) {
                                                        localItems += "<option value='" + row.value + "'>" + row.text + "</option>";
                                                    });
                                                    $("#ddlLocalidade").html(localItems);
                                                    $("#ddlLocalidade").val(f.localidadeId);
                                                })
                                                .fail(function () {
                                                    new PNotify({
                                                        title: 'Erro',
                                                        text: 'Não foi possível carregar as localidades.',
                                                        type: 'error'
                                                    });
                                                });
                                        }, 300);
                                    })
                                    .fail(function () {
                                        new PNotify({
                                            title: 'Erro',
                                            text: 'Não foi possível carregar os municípios.',
                                            type: 'error'
                                        });
                                    });
                            }, 300);
                        })
                        .fail(function () {
                            new PNotify({
                                title: 'Erro',
                                text: 'Não foi possível carregar os dados do fomento.',
                                type: 'error'
                            });
                        })
                        .always(function () {
                            self.ShowLoad(false, "pFiltro");
                        });
                });
            }

            //self.GetPesquisaAluno();

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
        DeleteAluno: function (id) {
            var url = "Aluno/Delete/" + id;
            $("#deleteAlunoHref").prop("href", url);
        },
        CarteirinhaAluno: function (id) {
            var self = this;

            axios.get("Aluno/GetAlunoById/?id=" + id)
                .then(result => {
                    self.editDto.Id = result.data.id;
                    self.editDto.FomentoId = result.data.fomentoId;
                    self.editDto.Nome = result.data.nome;
                    self.editDto.Status = result.data.status;
                    self.editDto.Email = result.data.email;
                    self.editDto.Sexo = result.data.sexo;
                    self.editDto.Cpf = result.data.cpf;
                    self.editDto.Cep = result.data.cep;
                    self.editDto.DtNascimento = result.data.dtNascimento;
                    self.editDto.MunicipioEstado = result.data.municipioEstado;
                    self.editDto.NomeLocalidade = result.data.nomeLocalidade;
                    self.editDto.Telefone = result.data.celular;

                    if (result.data.celular === "0" || result.data.celular === "" || result.data.celular === null) {
                        self.editDto.Telefone = "Não informado";
                    }
                    else {
                        self.editDto.Telefone = result.data.celular;
                    }

                    if (result.data.image == null && result.data.sexo === "Feminino") {
                        const studentPhoto = document.querySelector('.student-photo');
                        if (studentPhoto) {
                            studentPhoto.style.objectFit = "inherit";
                        }
                        self.editDto.Image = 'assets/images/menina.png';
                    } else if (result.data.image == null && result.data.sexo === "Masculino") {
                        const studentPhoto = document.querySelector('.student-photo');
                        if (studentPhoto) {
                            studentPhoto.style.objectFit = "inherit";
                        }
                        self.editDto.Image = 'assets/images/menino.png';
                    }
                    else {
                        const studentPhoto = document.querySelector('.student-photo');
                        if (studentPhoto) {
                            studentPhoto.style.objectFit = "cover";
                        }
                        self.editDto.Image = 'data:image/jpeg;base64,' + result.data.image;
                    }

                    if (result.data.cpf === "0" || result.data.cpf === "" || result.data.cpf === null) {
                        self.editDto.Cpf = "Não informado";
                    }
                    else {
                        self.editDto.Cpf = result.data.cpf;
                    }

                    if (result.data.modalidades === "0" || result.data.modalidades === "" || result.data.modalidades === null) {
                        self.editDto.modalidades = "Modalidade não informada";
                    }
                    else {
                        self.editDto.Modalidades = result.data.modalidades;
                    }

                    self.editDto.QRCode = 'data:image/jpeg;base64,' + result.data.qrCode;

                    // Após ter o fomentoId, buscar o modelo da carteirinha
                    return axios.get("Aluno/GetModeloCarteirinhaByFomento?fomentoId=" + result.data.fomentoId);
                })
                .then(modeloResult => {
                    // Atualizar o background da frente com o nome da imagem retornado
                    if (modeloResult.data) {
                        const frenteElement = document.querySelector('#frente');
                        if (frenteElement) {
                            frenteElement.style.backgroundImage = `url(/assets/styles_Carteirinha/modelos/${modeloResult.data.nomeImagemFrente}.png)`;
                        }

                        // Atualizar o background do verso com o nome da imagem retornado

                        const versoElement = document.querySelector('#verso');
                        if (versoElement) {
                            versoElement.style.backgroundImage = `url(/assets/styles_Carteirinha/modelos/${modeloResult.data.nomeImagemVerso}.png)`;
                        }
                    }
                })
                .catch(error => {
                    Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                });
        },
        GetPesquisaAluno: function () {

            var self = this;
            self.ShowLoad(true, "pResult");

            var obj = {
                FomentoId: $("#ddlFomento").val(),
                Estado: $("#ddlEstado").val(),
                MunicipioId: $("#ddlMunicipio").val(),
                LocalidadeId: $("#ddlLocalidade").val(),
                ProfissionalId: $("#ddlProfissional").val(),
                DeficienciaId: $("#ddlDeficiencia").val(),
                Nome: $("#nome").val(),
                Matricula: $("#matricula").val(),
                Etnia: $("#ddlEtnia").val(),
                Sexo: $("#ddlSexo").val(),
                PossuiFoto: $("#possuiFoto").is(":checked")
            }

            let axiosConfig = {
                headers: {
                    'Content-Type': 'application/json;charset=UTF-8',
                    "Access-Control-Allow-Origin": "*",
                }
            };

            axios.post("Aluno/GetAlunosByFilter", obj, axiosConfig).then(result => {
                self.ShowLoad(false, "pResult");
            });

            self.ShowLoad(false, "pResult");
        },
        DeleteAluno: function (id) {
            var url = "Aluno/Delete/" + id; 
            $("#deleteAlunoHref").prop("href", url);
        }
    }
});
var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteAlunoId"]').attr('value', id);
        $('#mdDeleteAluno').modal('show');
        vm.DeleteAluno(id)
    },
    CarterinhaModal: function (id) {
        $('input[name="carteirinhaAlunoId"]').attr('value', id);
        $('#mdCarteirinhaAluno').modal('show');
        vm.CarteirinhaAluno(id);
    }
};