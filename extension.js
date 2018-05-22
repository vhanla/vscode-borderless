const vscode = require('vscode')
const shell = require('node-powershell')

function activate(context) {
	const path = context.asAbsolutePath('./ToggleBorder.cs')
	const ps = new shell({
		executionPolicy: 'ByPass',
		noProfile: true
	})
	context.subscriptions.push(ps)
	ps.addCommand(`Add-Type -Path '${path}'`)

	function toggleBorder(visible) {
		ps.addCommand(`[CBS]::ToggleBorder(${process.pid}, ${visible})`)
		ps.invoke().then(res => {
			// console.log(res)
			console.log(`Borderless: set visible ${visible}`)
			//config.update('borderless', visible, true)
		}).catch(err => {
			console.log(err)
			// vscode.window.showErrorMessage(`Borderless Error: ${err}`)
		})
	}

	context.subscriptions.push(vscode.commands.registerCommand('borderless.on', () => {
		// vscode.window.showInformationMessage('Title Bars enabled.')
		toggleBorder(0)
	}))

	context.subscriptions.push(vscode.commands.registerCommand('borderless.off', () => {
		toggleBorder(1)
	}))
}
exports.activate = activate

function deactivate() {}

exports.deactivate = deactivate

