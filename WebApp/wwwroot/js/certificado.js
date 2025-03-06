var vm = new Vue({
    el: "#vCertificado ",
    data: {
        loading: false,
        editDto: { Id: "", TipoCurso: "", Curso: "", ImagemFrente: "", HtmlFrente: "", ImagemVerso: "", HtmlVerso: "", NomeImagemFrente: "", NomeImagemVerso: "", Status: true }
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

            if (formid === "formEditCertificado") {

                $("#formEditCertificado ").validate({
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

            if (formid === "formCertificado") {


                $("#formCertificado").validate({
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
        DeleteCertificado: function (id) {
            var url = "Certificado/Delete/" + id;
            $("#deleteCertificadoHref").prop("href", url);
        },
        EditCertificado: function (id) {
            var self = this;

            axios.get("Certificado/GetCertificadoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.TipoCurso = result.data.tipoCurso;
                self.editDto.Curso = result.data.curso;
                self.editDto.HtmlFrente = result.data.htmlFrente;
                self.editDto.HtmlVerso= result.data.htmlVerso;
                self.editDto.Status = result.data.status;

                self.editDto.ImagemFrente = "\\Certificados\\" + result.data.imagemFrente;
                if (result.data.imagemVerso && result.data.imagemVerso.includes("\Certificados")) {
                    self.editDto.ImagemVerso = "\\Certificados\\" + result.data.imagemVerso;
                } else {
                    self.editDto.ImagemVerso = null;
                }


            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        Certificado: function (id) {
            var self = this;

            axios.get("Certificado/GetCertificadoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.TipoCurso = result.data.tipoCurso;
                self.editDto.Curso = result.data.curso;
                self.editDto.HtmlFrente = result.data.htmlFrente;
                self.editDto.HtmlVerso = result.data.htmlVerso;
                self.editDto.Status = result.data.status;

                self.editDto.ImagemFrente = "\\Certificados\\" + result.data.imagemFrente;
                if (result.data.imagemVerso && result.data.imagemVerso.includes("\Certificados")) {
                    self.editDto.ImagemVerso = "\\Certificados\\" + result.data.imagemVerso;
                } else {
                    self.editDto.ImagemVerso = null;
                }


                //if (result.data.celular === "0" || result.data.celular === "" || result.data.celular === null) {
                //    self.editDto.Telefone = "Não informado";
                //}
                //else {
                //    self.editDto.Telefone = result.data.celular;
                //}
                //if (result.data.image == null && result.data.sexo == "Feminino") {
                //    self.editDto.Image = 'assets/images/menina.jpg';
                //} else if (result.data.image == null && result.data.sexo == "Masculino") {
                //    self.editDto.Image = 'assets/images/menino.jpg';
                //} else {
                //    self.editDto.Image = 'data:image/jpeg;base64,' + result.data.image;
                //}
                //if (result.data.cpf === "0" || result.data.cpf === "" || result.data.cpf === null) {
                //    self.editDto.Cpf = "Não informado";
                //}
                //else {
                //    self.editDto.Cpf = result.data.cpf;
                //}
                //if (result.data.modalidadeLinhaAcao === "0" || result.data.modalidadeLinhaAcao === "" || result.data.modalidadeLinhaAcao === null) {
                //    self.editDto.ModalidadeLinhaAcao = "Modalidade / Linha de Ação (não informado)";
                //}
                //else {
                //    self.editDto.ModalidadeLinhaAcao = result.data.modalidadeLinhaAcao;
                //}
                //self.editDto.QRCode = 'data:image/jpeg;base64,' + result.data.qrCode;



                //var text = 'http://front.hml.dnadobrasil.org.br/Identity/Account/ControlePresenca?alunoId=' + self.editDto.Id;

                //$('#qr').ClassyQR({
                //    create: true,// signals the library to create the image tag inside the container div.
                //    type: 'text',// text/url/sms/email/call/locatithe text to encode in the QR. on/wifi/contact, default is TEXT
                //    text: text// the text to encode in the QR.
                //});

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteCertificadoId"]').attr('value', id);
        $('#mdDeleteCertificado').modal('show');
        vm.DeleteCertificado(id)
    },
    EditModal: function (id) {
        $('input[name="editCertificadoId"]').attr('value', id);
        $('#mdEditCertificado').modal('show');
        vm.EditCertificado(id)
    },
    CertificadoModal: function (id) {
        $('input[name="certificadoId"]').attr('value', id);
        $('#mdCertificado').modal('show');
        vm.Certificado(id);
    }
};