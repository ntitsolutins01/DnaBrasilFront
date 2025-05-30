var vm = new Vue({
    el: "#vQuestaoEad",
    data: {
        loading: false,
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            //busca o id do form
            var formid = $('form')[1].id;

            if (formid === "formQuestaoEad") {


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

                $("#ddlTipoCurso").change(function () {
                    var tipoCursoId = $("#ddlTipoCurso").val();

                    var sigla = $("#ddlTipoCurso").val();

                    var url = "../Curso/GetCursosAllByTipoCursoId";

                    $.getJSON(url,
                        { id: tipoCursoId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Curso</option>';
                                $("#ddlCurso").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlCurso").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Curso',
                                    text: 'Curso não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#ddlCurso").change(function () {
                    var cursoId = $("#ddlCurso").val();

                    var url = "../ModuloEad/GetModulosEadAllByCursoId";

                    $.getJSON(url,
                        { id: cursoId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Modulo</option>';
                                $("#ddlModuloEad").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlModuloEad").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Modulo',
                                    text: 'Modulo não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#ddlModuloEad").change(function () {
                    var moduloEadId = $("#ddlModuloEad").val();

                    var url = "../Aula/GetAulasAllByModuloEadId";

                    $.getJSON(url,
                        { id: moduloEadId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Aula</option>';
                                $("#ddlAula").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlAula").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Aula',
                                    text: 'Aulas não encontradas.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#ddlTipoResposta").change(function () {
                    if ($(".wrapperResposta").find("*").length > 0) {
                        $(".wrapperResposta").empty();
                        $(".wrapperRespostaInfo").empty();
                    }
                });

                

                $("#formQuestaoEad").validate({
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
        addTextoImagem: function () {
            var self = this;

            var elem = $("select[name^=ddlTipoTextoImagem]").length;

            if (elem > 3) {
                return false;
            }

            var ddl = 'ddlTipoTextoImagem' + $("select[name^='ddlTipoTextoImagem']").length;

            $('.wrapper').append(`
                <div class="col-sm-4" style="padding-top: 15px;" id="divDdl`+elem+`">
	                <select class="form-control populate select2" name="`+ ddl + `" id="` + ddl + `"
			                title="Por favor selecione um sexo." required style="width:100%">
		                <option value="">Selecionar tipo texto ou imagem</option>
		                <option value="T">TEXTO</option>
		                <option value="I">IMAGEM</option>
	                </select>
                </div>
            `);

            self.setSelected2(ddl);

            $("#"+ddl).change(function () {
                var value = $("#" + ddl).val();

                var innerHTML = '';

                switch (value) {

                    case 'T':

                        $('.wrapper').append(`
                                            <div class="col-sm-7" style="padding-top: 15px;" id="div`+elem+`">
                                                <textarea class="form-control" rows="3" maxlength="500" name="texto` + elem + `" id="texto` + elem + `"></textarea>
                                            </div>
                                            <div class="col-sm-1" style="padding-top: 15px;" id="btn`+ elem +`">
                                                <a type="button" class="ml-xs btn btn-danger" href="javascript:(crud.DelTextoImagem('divDdl`+ elem + `','div` + elem + `','btn` + elem +`'))">
                                                    <i class="fa fa-minus"></i>
                                                </a>
                                            </div>
                                        `);
                        break;

                    case 'I':
                        $('.wrapper').append(`
                                            <div class="col-sm-7" style="padding-top: 15px;" id="div`+ elem +`">
                                                <div class="fileupload fileupload-new" data-provides="fileupload">
                                                    <div class="input-append">
                                                        <div class="uneditable-input">
                                                            <i class="fa fa-file fileupload-exists"></i>
                                                            <span class="fileupload-preview"></span>
                                                        </div>
                                                        <span class="btn btn-default btn-file">
                                                            <span class="fileupload-exists">Alterar</span>
                                                            <span class="fileupload-new">Selecionar Imagem</span>
                                                            <input type="file" id="imagem` + elem + `" name="imagem` + elem + `" />
                                                        </span>
                                                        <a href="#" class="btn btn-default fileupload-exists" data-dismiss="fileupload">Remover</a>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-1" style="padding-top: 15px;" id="btn`+elem+`">
                                                <a type="button" class="ml-xs btn btn-danger"
                                                href="javascript:(crud.DelTextoImagem('divDdl`+ elem + `','div` + elem + `','btn` + elem +`'))">
                                                    <i class="fa fa-minus"></i>
                                                </a>
                                            </div>
                                        `);
                        break;

                    default:
                        break;
                }
            });
        },
        setSelected2: function (ddl) {
            var $select = $("#"+ddl).select2({
                allowClear: true
            });

            $("#" + ddl).each(function () {
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
        },
        addTipoResposta: function () {
            var self = this; 

            var tipoResposta = $("#ddlTipoResposta").val();

            switch (tipoResposta) {
                case 'A':

                    var elem = $("input[name^=alternativa]").length;

                    if (elem > 4) {
                        return false;
                    }
                     

                    const letter = String.fromCharCode(65 + elem);
                    var alternativa = 'alternativa' + letter;
                    var div = 'div' + letter;

                    $('.wrapperResposta').append(`
                                            <div class="form-group row" name="`+ div + `" id="` + div +`">
                                                <label class="col-sm-1 control-label" style="padding-bottom: 15px;">`+ letter +` <span class="required">*</span></label>
                                                <div class="col-sm-9">
                                                    <input type="text" name="`+ alternativa + `" id="` + alternativa +`" maxlength="200" 
                                                    class="form-control" required title="Por favor informe o texto da alternativa." />
                                                </div>
                                                <div class="radio-custom radio-success">
													<input type="radio" id="radio`+ alternativa + `" name="radioAlternativa">
													<label for="radio`+ alternativa + `"> </label>
												</div>
                                            </div>
                                            
                                        `);
                    break;
                case 'D':

                    var elem = $("input[name^=dissertativa]").length;

                    if (elem > 0) {
                        return false;
                    }

                    $('.wrapperResposta').append(`
                        <label class="col-sm-3 control-label text-sm-right pt-2">Quantidade de caracteres <span class="required">*</span></label>
					                <div class="col-sm-2">
                            <input type="number" id="dissertativa" name="dissertativa" class="form-control" maxlength="4" required
							                   title="Por favor informe a quantidade de caracteres." />
					                </div>
                    `);
                    break;
                case 'M':

                    var elem = $("input[name^=multiplo]").length;

                    if (elem > 4) {
                        return false;
                    }

                    let valueMultiplo = Math.pow(2, elem);

                    var multiplo = 'multiplo' + valueMultiplo;
                    var checkbox = 'ckMulti' + valueMultiplo;
                    var div = 'div' + valueMultiplo;

                    $('.wrapperResposta').append(`
                                            <div class="form-group row" name="`+ div + `" id="` + div + `">
                                                <div class="col-sm-1">
                                                    <div class="checkbox-custom checkbox-default">
			                                            <input id="`+ checkbox + `" name="` + checkbox + `" type="checkbox" value="` + valueMultiplo + `" />
			                                            <label for="`+ checkbox + `"></label>
		                                            </div>
                                                </div>
                                                <div class="col-sm-9">
                                                    <input type="text" name="`+ multiplo + `" id="` + multiplo + `" maxlength="200" 
                                                    class="form-control" required title="Por favor informe o texto da multipla escolha." />
                                                </div>
                                            </div>
                                        `);

                    if ($("#infoMulti").length === 0) {

                        $('.wrapperRespostaInfo').append(`
                            <code id="infoMulti" name="infoMulti">.panel-featured.panel-featured-primary</code>
                                            `);
                    }


                    break;
                default:
                    Site.Notification("Atenção!", "Por favor selecione o tipo de resposta", "warning", 1);
                    break;
            }
        },
        DeleteQuestaoEad: function (id) {
            var url = "QuestaoEad/Delete/" + id;
            $("#deleteQuestaoEadHref").prop("href", url);
        },
        delTextoImagem: function (ddl, textoImagem, btn) {
            var self = this; 

            $("#" + ddl).remove();

            $("#" + textoImagem).remove();

            $("#" + btn).remove();
        },
        delTipoResposta: function (tipoResposta, elemento, label) {
            var self = this;

            $(".wrapperResposta").empty();
            $(".wrapperRespostaInfo").empty();
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteQuestaoEadId"]').attr('value', id);
        $('#mdDeleteQuestaoEad').modal('show');
        vm.DeleteQuestaoEad(id)
    },
    DelTipoResposta: function (tipoResposta, elemento) {
        vm.delTipoResposta(tipoResposta, elemento);
    },
    DelTextoImagem: function (ddl, textoImagem, btn) {
        vm.delTextoImagem(ddl, textoImagem, btn);
    }
};