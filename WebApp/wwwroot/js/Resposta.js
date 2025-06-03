var vm = new Vue({
    el: "#formResposta",
    data: {
        loading: false,
        editDto: { Id: "", Questionario: "", Resposta: "", Descricao: "", TipoLaudo: "", ValorPesoResposta: "", Vocacional:true, Educacional:false }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';
            var formid = $('form')[1].id;

            //mascara dos inputs
            var valorPeso = $("#valorPeso");
            valorPeso.mask('00.00', { reverse: true });

            if (formid === "formResposta") {
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


                $("#formResposta").validate({
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

                $("#ddlTipoLaudo").change(function () {

                    var id = $("#ddlTipoLaudo").val();

                    if (id === "10") {
                        $("#dvEducacional").show();
                    } else {
                        $("#dvEducacional").hide();
                    }

                    var url = "../Questionario/GetQuestionariosByTipoLaudo?id=" + id;

                    var ddlSource = "#ddlQuestionario";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                
                                var items = '<option value="">Selecionar Questionario</option>';
                                $("#ddlQuestionario").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlQuestionario").html(items);
                            }
                            else {
                                
                                new PNotify({
                                    title: 'Usuario',
                                    text: "Não existem questionários cadastrados para esse tipo de laudo.",
                                    type: 'warning'
                                });
                            }
                        });
                });
            }

            if (formid === "formEditResposta") {


                //mascara dos inputs
                var valorPesoDecimal = $("#valorPesoDecimal");
                valorPesoDecimal.mask('0.00', { reverse: false });


                $("#formEditResposta").validate({
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
        DeleteResposta: function (id) {
            var url = "Resposta/Delete/" + id;
            $("#deleteRespostaHref").prop("href", url);
        },
        EditResposta: function (id) {
            var self = this;

            var tipoLaudo = $("#ddlTipoLaudo").val();

            if (tipoLaudo === "10") {
                $("#dvEducacional").show();
            } else {
                $("#dvEducacional").hide();
            }

            axios.get("Resposta/GetRespostaById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Questionario = result.data.pergunta;
                self.editDto.TipoLaudo = result.data.nomeTipoLaudo;
                self.editDto.Resposta = result.data.respostaQuestionario;
                self.editDto.Descricao = result.data.descricao;
                self.editDto.ValorPesoResposta = result.data.valorPesoResposta;
                if (result.data.nomeTipoLaudo === 'Vocacional') {
                    self.editDto.Vocacional = true;
                }
                else if (result.data.nomeTipoLaudo === 'Educacional') {
                    self.editDto.Educacional = true;
                }
                else {
                    self.editDto.Vocacional = false;
                    self.editDto.Educacional = false;
                }

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="RespostaId"]').attr('value', id);
        $('#mdDeleteResposta').modal('show');
        vm.DeleteResposta(id)
    },
    EditModal: function (id) {
        $('input[name="RespostaId"]').attr('value', id);
        $('#mdEditResposta').modal('show');
        vm.EditResposta(id)
    }
};