NOME_TOKEN = "BearerToken";

var Site = {
    ResultAxio: function (result, path) {
        switch (result.status) {
            case 400: /*Bad Request*/
                Site.Notification("400 Requisição inválida", "O pedido não pode ser entregue devido à sintaxe incorreta.", "error");
                break;
            case 409: /*Conflict*/
                Site.Notification("409 Conflito", result.responseJSON, "warning");
                break;

            case 500: /*Internal Server Error*/
                Site
                    .Notification("500 Erro interno do servidor (Internal Server Error)", "Indica um erro do servidor ao processar a solicitação. " +
                        "Na grande maioria dos casos está relacionada as permissões dos arquivos ou pastas " +
                        "do software ou script que o usuário tenta acessar e não foram configuradas no momento " +
                        "da programação/construção do site ou da aplicação.", "error");
                break;

            case 204: /*OK*/
                Site.Notification("204 Excluido", "Exclusão realizado com sucesso!", "success");
                break;

            case 200: /*OK*/
                Site.Notification("200 Alterado", "Alteração realizado com sucesso!", "success");
                break;

            case 201: /*OK*/
                Site.Notification("201 Criado", "Cadastrado realizado com sucesso!", "success");
                break;

        }
    },
    Notification: function (title, text, style, type) {
        /*
    Positions
    */
        var stack_topleft = { "dir1": "down", "dir2": "right", "push": "top" };
        var stack_bottomleft = { "dir1": "right", "dir2": "up", "push": "top" };
        var stack_bottomright = { "dir1": "up", "dir2": "left", "firstpos1": 15, "firstpos2": 15 };
        var stack_bar_top = { "dir1": "down", "dir2": "right", "push": "top", "spacing1": 0, "spacing2": 0 };
        var stack_bar_bottom = { "dir1": "up", "dir2": "right", "spacing1": 0, "spacing2": 0 };
        var opts = {};
        switch (type) {
            case 1:
                opts = {
                    title: title,
                    text: text,
                    type: style,
                    addclass: 'stack-bottomright',
                    stack: stack_bottomright
                };
                break;
            case 2:
                opts = {
                    title: title,
                    text: text,
                    type: style,
                    addclass: 'stack_bar_top',
                    stack: stack_bar_top
                };
                break;
            default:
                opts = {
                    title: title,
                    text: text,
                    type: style,
                    addclass: 'stack-bar-top',
                    stack: stack_bar_top,
                    width: "100%"
                };
        }

        new PNotify(opts);
    },
    NotificationClose: function () {
        PNotify.removeAll();
    },
    Delay: function (time, newState, href) {
        setTimeout(function () {
            if (newState === -1) {
                window.location.href = href;
            }
        }, time);
    },
    SetDataLocalStorage: function (key, value) {
        window.localStorage.setItem(key, JSON.stringify(value));
    },
    ClearDataLocalStorage: function (key) {
        window.localStorage.removeItem(key);
    },
    SetLocalStorageData: function (key, value) {
        if (value === null) {
            // Remove o item
            window.localStorage.removeItem(key);
        } else {
            if (NOME_TOKEN === key) {
                window.localStorage.setItem(key, JSON.stringify(value));
            } else {
                window.localStorage.setItem(key, JSON.stringify(value));
            }
        }
    },
    GetLocalStorageData: function (key) {
        return JSON.parse(window.localStorage.getItem(key));
    },
    ClearLocalStorageData: function () {
        window.localStorage.clear();
    },
    LogOut: function () {
        Site.ClearLocalStorageData();

        window.location.href = "/Login/SignOut";
    },
    GetUrlVars: function () {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    GetUrlVar: function (name) {
        return Site.GetUrlVars()[name];
    }
};
var Storage = {
    GetPessoa: function (key) {
        var result = Site.GetLocalStorageData(key);
        if (result === null) {
            return 'testando storage';
            //Site.ClearLocalStorageData();
            //window.location.href = "/Login";
        } else {
            return JSON.parse(result.Pessoa);
        }
    }

};

var Iframe = {
    SetIframe: function () {
        this.style.height = this.contentDocument.body.scrollHeight + 'px';
    }
};

var Modal = {
    ModalFluxoTipoDocumento: function (titulo, mensagem) {

        document.getElementById('tituloFluxoTipoDocumento').innerHTML = titulo;
        document.getElementById('msgFluxoTipoDocumento').innerHTML = mensagem;

        $("#myFluxoTipoDocumentoModal").modal('show');
    },
    ModalListAlerta: function (titulo, mensagem) {

        document.getElementById('tituloListAlerta').innerHTML = titulo;
        document.getElementById('msgListAlerta').innerHTML = mensagem;

        $("#myListAlertaModal").modal('show');
    },
    ModalNoticias: function (titulo, mensagem) {

        document.getElementById('tituloNoticias').innerHTML = titulo;
        document.getElementById('msgNoticias').innerHTML = mensagem;

        $("#myNoticiasModal").modal('show');
    },
    ModalListDocumentosTramitacao: function (titulo, mensagem) {

        document.getElementById('tituloListDocumentosTramitacao').innerHTML = titulo;
        document.getElementById('msgListDocumentosTramitacao').innerHTML = mensagem;

        $("#myListDocumentosTramitacaoModal").modal('show');
    },
    MessageDefault: function (titulo, mensagem, modalDialog, headerCss, btnCss) {

        document.getElementById('tituloDefault').innerHTML = titulo;
        document.getElementById('msgDefault').innerHTML = mensagem;
        $("#headerDefault").addClass(headerCss);
        $("#btnDefault").addClass(btnCss);
        $("#dialogListUsuario").addClass(modalDialog);


        $('#myMsgDefaultModal').modal('show');
    },
    ModalListUsuario: function (titulo, mensagem, dialogListUsuario, headerCss, btnCss) {

        document.getElementById('tituloListUsuario').innerHTML = titulo;
        document.getElementById('msgListUsuario').innerHTML = mensagem;
        $("#headerListUsuario").addClass(headerCss);
        $("#btnListUsuario").addClass(btnCss);
        $("#dialogListUsuario").addClass(dialogListUsuario);


        $('#myListUsuarioModal').modal('show');
    },
    ModalListOrgao: function (titulo, mensagem, dialogListOrgao, headerCss, btnCss) {

        document.getElementById('tituloListOrgao').innerHTML = titulo;
        document.getElementById('msgListOrgao').innerHTML = mensagem;
        $("#headerListOrgao").addClass(headerCss);
        $("#btnListOrgao").addClass(btnCss);
        $("#dialogListOrgao").addClass(dialogListOrgao);


        $('#myListOrgaoModal').modal('show');
    },
    ModalListDonor: function (titulo, mensagem, dialogListDonor, headerCss, btnCss) {

        document.getElementById('tituloListDonor').innerHTML = titulo;
        document.getElementById('msgListDonor').innerHTML = mensagem;
        $("#headerListDonor").addClass(headerCss);
        $("#btnListDonor").addClass(btnCss);
        $("#dialogListDonor").addClass(dialogListDonor);


        $('#myListDonorModal').modal('show');
    },
    ModalFiltroProjetos: function (titulo, mensagem) {

        document.getElementById('tituloFiltroProjetos').innerHTML = titulo;
        document.getElementById('msgFiltroProjetos').innerHTML = mensagem;

        $("#myFiltroProjetosModal").modal('show');
    },
    ModalSessionExpires: function (titulo, mensagem, pathName) {

        document.getElementById('tituloSessionExpires').innerHTML = titulo;
        document.getElementById('msgSessionExpires').innerHTML = mensagem;

        $("#myMsgSessionExpiresModal").modal('show');

        $('#myMsgSessionExpiresModal').on('click', function (evt) {
            window.location.href = pathName;

        });
    },
    MessageBoxPergunta: function (titulo, mensagem, pathName) {

        document.getElementById('tituloPergunta').innerHTML = "<i class='fa fa-fw fa-question-circle'></i>&nbsp;&nbsp;" + titulo;
        document.getElementById('msgPergunta').innerHTML = mensagem;

        $('#myMsgPerguntaModal').modal('show');
    },
    MessageBoxDelete: function (titulo, mensagem, pathName) {

        document.getElementById('tituloDelete').innerHTML = "<i class='fa fa-fw fa-trash'></i>&nbsp;&nbsp;" + titulo;
        document.getElementById('msgDelete').innerHTML = mensagem;


        $('#myMsgModalDelete').modal('show');
    }
};

var animation = {
    Execute: function (x, id) {
        $(id).removeClass().addClass(x + ' animated').one(
            'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend',
            function () {
                $(this).removeClass();
            });
    }
};

var appContext = {
    createCORSRequest: function () {
        alert("Vem do main");
    }
};

var Mask = {
    //Máscaras para formatar DATA, VALOR E ETC
    mascara: function (objeto, funcao) {
        obj = objeto;
        fun = funcao;
        setTimeout("Mask.exec_mascara()", 1);
    },
    exec_mascara: function () {
        obj.value = fun(obj.value);
    },
    format_moeda: function (v) { // formato: 9.999.999,99
        v = v.replace(/\D/g, "");
        v = v.replace(/(\d)(\d{2})$/, "$1,$2");
        v = v.replace(/(\d+)(\d{3},\d{2})$/g, "$1.$2");
        var qtd = (v.length - 3) / 3;
        var cont = 0;
        while (qtd > cont) {
            cont++;
            v = v.replace(/(\d+)(\d{3}.*)/, "$1.$2");
        }

        if (v.length >= 3) {
            v = v.replace(/^(0+)(\d)/g, "$2");
        }

        return v;
    },
    formatReal: function (val) {
        var formatter = new Intl.NumberFormat('pt-BR',
            {
                currency: 'BRL',
                minimumFractionDigits: 2
            });

        var result = formatter.format(val);

        return result;
    },
    formatMoedaPrefixWithType: function (val, type) {

        var result = val;

        if (type === 3) {
            result = this.formatRealPrefix(val);
        }

        if (type === 4) {
            result = this.formatDolarPrefix(val);
        }

        return result;
    },
    formatRealPrefix: function (val) {
        var formatter = new Intl.NumberFormat('pt-BR',
            {
                style: 'currency',
                currency: 'BRL',
                minimumFractionDigits: 2
            });

        var result = formatter.format(val);

        return result;
    },
    formatDolarPrefix: function (val) {
        var formatter = new Intl.NumberFormat('pt-BR',
            {
                style: 'currency',
                currency: 'USD',
                minimumFractionDigits: 2
            });

        var result = formatter.format(val);

        return result.replace("S", "");
    },
    format_float: function (v) // formato: 9999999.99 número decimal com duas casas após a vírgula
    {
        v = v.replace(/\D/g, "");
        v = v.replace(/(\d)(\d{2})$/, "$1.$2");
        //v = v.replace('.', ',');
        return v;
    },
    unformatCpf: function (cpf) {
        return cpf.value.replace(/\D/g, '');
    }
};

var Helper = {
    FindByHandle: function (source, id) {
        for (var i = 0; i < source.length; i++) {
            if (source[i].Handle === id) {
                return source[i];
            }
        }
        throw "Couldn't find object with id: " + id;
    },
    FindIndexByHandle: function (source, id) {
        for (var i = 0; i < source.length; i++) {
            if (source[i].Handle === id) {
                return i;
            }
        }
        throw "Couldn't find object with id: " + id;
    }
};