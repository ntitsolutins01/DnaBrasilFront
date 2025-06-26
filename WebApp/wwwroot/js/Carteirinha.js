var vm = new Vue({
    el: "#vCarteirinha",
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

            //var formid = $('form')[1].id;

            var self = this;

            const queryString = window.location.pathname;
            const urlParams = queryString.split('/');
            
            axios.get("../../Aluno/GetAlunoById/?id=" + urlParams[3])
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
                    return axios.get("../../Aluno/GetModeloCarteirinhaByFomento?fomentoId=" + result.data.fomentoId);
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
        }
    }
});
var crud = {
};